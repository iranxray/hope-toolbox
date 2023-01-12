namespace Xray.Hope.Web.Shared.DTO.CheckServerConnection
{
    public class CheckServerConnectionResponse
    {
        public bool IsSshPortOpen { get; set; }

        public bool CanConnectSsh { get; set; }
    }
}
