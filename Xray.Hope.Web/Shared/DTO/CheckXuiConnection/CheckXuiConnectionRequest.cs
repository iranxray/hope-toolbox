namespace Xray.Hope.Web.Shared.DTO.CheckServerConnection
{
    public class CheckXuiConnectionRequest
    {
        public string ServerAddress { get; set; }

        public int XuiPort { get; set; }

        public string XuiUsername { get; set; }

        public string XuiPassword { get; set; }
    }
}
