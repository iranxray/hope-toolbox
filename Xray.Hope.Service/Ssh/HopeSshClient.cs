using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System.Text;

namespace Xray.Hope.Service.Ssh
{
    public class HopeSshClient
    {
        private readonly ILogger<HopeSshClient> _logger;
        public HopeSshClient(ILogger<HopeSshClient> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Checks whether ssh connection can be made given the connection options.
        /// </summary>
        /// <param name="sshConnectionOptions"></param>
        /// <returns>Returns true if ssh connection is successful</returns>
        public bool CanConnect(SshConnectionOptions sshConnectionOptions)
        {
            _ = sshConnectionOptions ?? throw new ArgumentNullException(nameof(sshConnectionOptions));

            if (!sshConnectionOptions.IsValid())
            {
                throw new Exception("The provided ssh connection params are not valid.");
            }

            _logger.LogInformation("Connect to the server via ssh on port `{port}`.", sshConnectionOptions.Port);

            try
            {
                using var client = CreateSshClient(sshConnectionOptions);
                client.Connect();
                client.Disconnect();

                _logger.LogInformation("Successfully connected to the server via ssh on port `{port}`.", sshConnectionOptions.Port);

                return true;
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to connect to the server via ssh.");
                return false;
            }
        }

        /// <summary>
        /// Executes a script through ssh shell.
        /// </summary>
        /// <param name="sshConnectionOptions"></param>
        /// <param name="script"></param>
        /// <param name="onReceivedFunc">Optional. A callback to be called when the shell's output is received.</param>
        /// <param name="onCompleted">Optional. A callback to be called when the script execution ends.</param>
        /// <param name="onCompleted">Optional. Times out the execution.</param>
        public async Task ExecuteScriptAsync(
           SshConnectionOptions sshConnectionOptions,
           string script,
           Action<string> onReceivedFunc = null,
           Action onCompleted = null,
           CancellationToken cancellationToken = default)
        {
            _ = script ?? throw new ArgumentNullException(nameof(script));
            _ = sshConnectionOptions ?? throw new ArgumentNullException(nameof(sshConnectionOptions));

            if (!sshConnectionOptions.IsValid())
            {
                throw new Exception("The provided ssh connection params are not valid.");
            }

            // Shell is an infinite stream. We create a terminator to find out when the script execution finishes.
            var terminator = Guid.NewGuid().ToString();
            script += Environment.NewLine + Echo(terminator);

            var executingIsDone = false;

            using var client = CreateSshClient(sshConnectionOptions);

            client.Connect();

            // vt100 is a valid terminal type in Linux.
            using var sshShellStream = client.CreateShellStream("vt100", 0, 0, 0, 0, 1000000);

            // For sending commands.
            var swInput = new StreamWriter(sshShellStream);
            swInput.AutoFlush = true;

            // For receiving output.
            var srOutput = new StreamReader(sshShellStream);

            sshShellStream.DataReceived += (sender, e) =>
            {
                var line = srOutput.ReadToEnd();

                // Checks the terminate conditions.
                if (line.Contains(terminator) && !line.Contains("echo"))
                {
                    executingIsDone = true;
                    onCompleted?.Invoke();

                    return;
                }

                onReceivedFunc?.Invoke(line);
            };

            // Execute the script.
            swInput.WriteLine(script);

            // Wait until script execution ends.
            while (!executingIsDone)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("The script execution via ssh timed out.");
                    throw new TaskCanceledException("The execution timed out.");
                }

                await Task.Delay(1000);
            }

            _logger.LogInformation("Successfully ran the script on the server.");
        }

        private static string Echo(string terminator)
        {
            return $"echo \"{terminator}\"";
        }

        private static SshClient CreateSshClient(SshConnectionOptions sshConnectionOptions)
        {
            AuthenticationMethod authenticationMethod = default;

            // Create authentication method according to the provided credentials.
            if (!string.IsNullOrEmpty(sshConnectionOptions.Password) && string.IsNullOrEmpty(sshConnectionOptions.PrivateKeyContent))
            {
                authenticationMethod = new PasswordAuthenticationMethod(sshConnectionOptions.Username, sshConnectionOptions.Password);
            }
            else if (!string.IsNullOrEmpty(sshConnectionOptions.PrivateKeyContent))
            {
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sshConnectionOptions.PrivateKeyContent));
                authenticationMethod = new PrivateKeyAuthenticationMethod(sshConnectionOptions.Username, new PrivateKeyFile(stream));
            }
            else
            {
                throw new ArgumentNullException("Either password or private key should be present");
            }

            var connection = new ConnectionInfo(
              sshConnectionOptions.Host,
              sshConnectionOptions.Port,
              sshConnectionOptions.Username,
              authenticationMethod);

            return new SshClient(connection);
        }
    }
}
