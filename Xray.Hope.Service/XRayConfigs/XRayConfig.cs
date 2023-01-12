namespace Xray.Hope.Service.XRayConfigs
{
    public abstract record class XRayConfig
    {
        public string Host { get; set; }

        public string Id { get; set; }

        public string ClientId { get; set; }

        public string Remark { get; set; }

        public string Network { get; set; } = "tcp";

        public int Port { get; set; }

        public string Encryption { get; set; }

        public string Flow { get; set; }

        public string Sni { get; set; }

        public string PublicKeyFile { get; set; }

        public string PrivateKeyFile { get; set; }

        public string Protocol { get; internal set; }


        public Dictionary<string, string> Headers = new Dictionary<string, string>();


        public bool Recommended { get; set; }
        public string Description { get; set; }


        public abstract XRayConfigLink Link { get; }

        public abstract string CreateXrayConfig();
    }
}
