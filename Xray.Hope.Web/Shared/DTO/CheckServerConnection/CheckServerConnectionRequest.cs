namespace Xray.Hope.Web.Shared.DTO.CheckServerConnection
{
    public class CheckServerConnectionRequest
    {
        public string ServerAddress { get; set; }

        public string ServerUsername { get; set; }

        public string ServerPassword { get; set; }

        public string ServerPrivateKeyContent { get; set; }

        public int ServerSshPort { get; set; }
    }
}

