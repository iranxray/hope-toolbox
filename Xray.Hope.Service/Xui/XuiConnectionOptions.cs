namespace Xray.Hope.Service.Xui
{
    /// <summary>
    /// The required params for connecting to X-UI.
    /// </summary>
    public class XuiConnectionOptions
    {

        /// <summary>
        /// Server or domain or IP.
        /// </summary>
        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}