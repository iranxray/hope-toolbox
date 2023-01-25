using Xray.Hope.Service.XRayConfigs.Trojan;
using Xray.Hope.Service.XRayConfigs.VLESS;

namespace Xray.Hope.Service.XRayConfigs
{
    public class XRayConfigCollection
    {
        private static readonly Random random = new Random();

        private static TrojanConfig TrojanTls80 = new TrojanConfig
        {
            Encryption = "tls",
            Port = 80,
            Remark = "Trojan-Tls-80",
            Id = "TrojanTls80",
            Recommended = true,
        };

        private static TrojanConfig TrojanTls443 = new TrojanConfig
        {
            Encryption = "tls",
            Port = 443,
            Remark = "Trojan-Tls-443",
            Id = "TrojanTls443",
            Recommended = true,
        };

        private static TrojanConfig TrojanXtls443 = new TrojanConfig
        {
            Encryption = "xtls",
            Port = 443,
            Remark = "Trojan-Xtls-443",
            Flow = "xtls-rprx-direct",
            Id = "TrojanXtls443"
        };

        private static TrojanConfig TrojanTlsRandom = new TrojanConfig
        {
            Encryption = "tls",
            Port = random.Next(minValue: 20000, 60000),
            Remark = "Trojan-Tls-Random",
            Id = "TrojanTlsRandom"
        };

        private static TrojanConfig TrojanXtlsRandom = new TrojanConfig
        {
            Encryption = "xtls",
            Port = random.Next(minValue: 20000, 60000),
            Remark = "Trojan-Xtls-Random",
            Flow = "xtls-rprx-direct",
            Id = "TrojanXtlsRandom"
        };


        private static VlessConfig VlessTlsTCP80 = new VlessConfig
        {
            Encryption = "tls",
            Port = 80,
            Remark = "Vless-Tls-TCP-80",
            Network = "tcp",
            Id = "VlessTlsTCP80",
            Recommended = true,
        };

        private static VlessConfig VlessXtlsTCP80 = new VlessConfig
        {
            Encryption = "xtls",
            Flow = "xtls-rprx-direct",
            Port = 80,
            Remark = "Vless-Xtls-TCP-80",
            Network = "tcp",
            Id = "VlessXtlsTCP80",
            Recommended = true,
        };

        private static VlessConfig VlessTCP80 = new VlessConfig
        {
            Encryption = "",
            Flow = "",
            Port = 80,
            Remark = "Vless-TCP-80",
            Network = "tcp",
            Id = "VlessTCP80",
        };

        private static VlessConfig VlessCdn80 = new VlessConfig
        {
            Host = "arzdigital.com",
            Encryption = "",
            Flow = "",
            Port = 80,
            Remark = "Vless-cdn-80",
            Network = "ws",
            Id = "VlessCdn80",
            Headers = new Dictionary<string, string> { { "host", "@SNI" } },
            Recommended = true,
            Description = "برای نصب این کانفیگ باید که دامنه را در کلادفلر تعریف کرده باشید."
        };

        private static VlessConfig VlessTlsWs2083 = new VlessConfig
        {
            Encryption = "tls",
            Flow = "",
            Port = 2083,
            Remark = "Vless-Tls-Ws-2083",
            Network = "ws",
            Id = "VlessTlsWs2083"
        };

        private static VlessConfig VlessCdn8080 = new VlessConfig
        {
            Host = "arzdigital.com",
            Encryption = "",
            Flow = "",
            Port = 8080,
            Remark = "Vless-CDN-8080",
            Network = "ws",
            Id = "VlessCdn8080",
            Headers = new Dictionary<string, string> { { "host", "@SNI" } },
            Recommended = true,
            Description = "برای نصب این کانفیگ باید که دامنه را در کلادفلر تعریف کرده باشید."
        };

        private static VlessConfig VlessXtlsTcpRandom = new VlessConfig
        {
            Encryption = "xtls",
            Flow = "xtls-rprx-direct",
            Port = random.Next(20000, 60000),
            Remark = "Vless-Xtls-Tcp-Random",
            Network = "tcp",
            Id = "VlessXtlsTcpRandom"
        };

        public readonly IReadOnlyCollection<XRayConfig> All = new List<XRayConfig>
        {
            TrojanTls80,
            TrojanTls443,
            TrojanXtls443,
            TrojanTlsRandom,
            TrojanXtlsRandom,
            VlessXtlsTCP80,
            VlessTCP80,
            VlessCdn80,
            VlessTlsWs2083,
            VlessCdn8080,
            VlessXtlsTcpRandom,
            VlessTlsTCP80
        };
    }
}
