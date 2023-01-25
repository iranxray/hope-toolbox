using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace Xray.Hope.Service.Telnet
{
    public class TelnetClient
    {
        private readonly ILogger<TelnetClient> _logger;

        public TelnetClient(ILogger<TelnetClient> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Checks whether the port on the host is open.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        /// <returns>Returns true if the port is open.</returns>
        public async Task<bool> IsPortOpenAsync(string host, int port, TimeSpan timeout = default)
        {
            _ = host ?? throw new ArgumentNullException(nameof(host));

            try
            {
                using var timeoutCts = new CancellationTokenSource();
                timeoutCts.CancelAfter(timeout == default ? TimeSpan.FromSeconds(5) : timeout);

                using var client = new TcpClient();
                await client.ConnectAsync(host, port, timeoutCts.Token);

                return true;
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to check if port is open. Exception: `{exception}`", e.ToString());
                return false;
            }
        }
    }
}
