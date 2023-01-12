namespace Xray.Hope.Service.Ssh
{
    /// <summary>
    /// The required params for connecting to a remote server via ssh.
    /// </summary>
    public class SshConnectionOptions
    {
        public string Host { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string PrivateKeyContent { get; set; }

        public int Port { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Host)
                && !string.IsNullOrEmpty(Username)
                && !(string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(PrivateKeyContent))
                && Port >= 22;
        }
    }
}