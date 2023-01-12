namespace Xray.Hope.Web.Client.Models.Domain
{
    public class SshConnectionOptions
    {
        public string Host { get; set; }

        public int Port { get; set; } = 22;

        public string Username { get; set; }

        public string Password { get; set; }

        public string PrivateKeyPath { get; set; }

        public string PrivateKeyContent { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Host)
                && !string.IsNullOrEmpty(Username)
                && !(string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(PrivateKeyContent))
                && Port != 0;
        }
    }
}
