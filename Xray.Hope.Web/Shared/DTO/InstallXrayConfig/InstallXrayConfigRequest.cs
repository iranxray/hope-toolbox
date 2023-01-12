namespace Xray.Hope.Web.Shared.DTO.InstallXrayConfigRequest
{
    public class InstallXrayConfigRequest
    {
        public string XrayConfigId { get; set; }

        /// <summary>
        /// Server or domain or IP.
        /// </summary>
        public string Host { get; set; }

        public int XuiPort { get; set; }

        public string XuiUsername { get; set; }

        public string XuiPassword { get; set; }

        public int SshPort { get; set; } = 22;

        public string SshUsername { get; set; }

        public string SShPassword { get; set; }

        public string SshPrivateKeyContent { get; set; }
    }
}
