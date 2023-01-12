using System.Net;

namespace Xray.Hope.Service.Xui
{
    public class XuiInstallScriptGenerator
    {
        public const string TerminatorLine = "The X-UI is installed successfully on your machine 😊.";

        public string Generate(string serverAddress, XuiConnectionOptions xuiConnectionOptions)
        {
            var script = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Xui/scripts/install-xui.bash"));

            if (IPAddress.TryParse(serverAddress, out _))
            {
                // Create a self-signed cert.
                script += Environment.NewLine + Environment.NewLine + File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Xui/scripts/create-self-signed-cert.bash"));
            }
            else
            {
                // Create a valid cert. we assume it's a valid domain that is referring to the server's IP.
                script += Environment.NewLine + Environment.NewLine + File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Xui/scripts/create-valid-cert.bash"));
            }

            script += Environment.NewLine + Environment.NewLine + $"echo \"{TerminatorLine}\"";

            return script
              .Replace("$MYUSER", xuiConnectionOptions.Username)
              .Replace("$MYPASS", xuiConnectionOptions.Password)
              .Replace("$MYDOMAIN", serverAddress)
              .Replace("$MYPORT", xuiConnectionOptions.Port.ToString())
              .Replace("\r", ""); ;
        }
    }
}