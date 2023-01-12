namespace Xray.Hope.Web.Shared.DTO.ExecuteScripts
{
    public class ExecuteScriptsRequest
    {
        public string HubConnectionId { get; set; }

        public string ServerAddress { get; set; }

        public string ServerUsername { get; set; }

        public string ServerPassword { get; set; }

        public string ServerPrivateKeyContent { get; set; }

        public int ServerSshPort { get; set; }

        public string Script { get; set; }
    }
}
