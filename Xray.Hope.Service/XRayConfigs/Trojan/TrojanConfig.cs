using Microsoft.AspNetCore.WebUtilities;
using System.Net;

namespace Xray.Hope.Service.XRayConfigs.Trojan
{
    /// <summary>
    /// Represents Trojan inbound protocol.
    /// </summary>
    public record TrojanConfig : XRayConfig
    {
        private static readonly string Template = @"
                up= 0&
                down= 0&
                total= 0&
                remark= @REMARK&
                enable= true&
                expiryTime= 0&
                listen= &
                port= @PORT&
                protocol= trojan&
                settings= 
                {
                    ""clients"": [
                    {
                      ""password"": ""@PASSWORD"",
                      ""flow"": ""@FLOW""
                    }
                  ],
                  ""fallbacks"": []
                }&
                streamSettings= 
                {
                  ""network"": ""@NETWORK"",
                  ""security"": ""@ENCRYPTION"", 
                  ""xtlsSettings"": {
                    ""serverName"": ""@SNI"",
                    ""certificates"": [
                      {
                        ""certificateFile"": ""@PRIVATE_KEY_FILE"",
                        ""keyFile"": ""@PUBLIC_KEY_FILE""
                      }
                    ]
                  },
                  ""tlsSettings"": {
                    ""serverName"": ""@SNI"",
                    ""certificates"": [
                      {
                        ""certificateFile"": ""@PRIVATE_KEY_FILE"",
                        ""keyFile"": ""@PUBLIC_KEY_FILE""
                      }
                    ]
                  },
                  ""tcpSettings"": {
                    ""header"": {
                      @HEADER
                    }
                  }
                }&
                sniffing= 
                {
                  ""enabled"": true,
                  ""destOverride"": [
                    ""http"",
                    ""tls""
                  ]
                }";

        public TrojanConfig()
        {
            Protocol = XRayProtocol.Trojan;
            Network = "tcp";
        }

        public override XRayConfigLink Link
        {
            get
            {
                var configParams = new Dictionary<string, string>()
                {
                    { "security", Encryption },
                    { "flow", Flow },
                    { "host", Sni },
                };

                var uri = QueryHelpers
                    .AddQueryString($"trojan://{ClientId}@{Host}:{Port}", configParams)
                    + $"#{WebUtility.UrlEncode(Remark)}";

                return new XRayConfigLink
                {
                    Value = uri
                };
            }
        }

        public override string CreateXrayConfig()
        {
            return Template
            .Replace(" ", string.Empty)
                .Replace("@HEADER", Headers.Count() == 0
                    ? "\"type\": \"none\""
                    : Headers.Select(kv => $"\"{kv.Key}\": \"{kv.Value}\"").Aggregate((a, b) => a + Environment.NewLine + b))
                .Replace(Environment.NewLine, string.Empty)
                .Replace("@PASSWORD", ClientId)
                .Replace("@REMARK", Remark)
                .Replace("@NETWORK", Network)
                .Replace("@PORT", Port.ToString())
                .Replace("@ENCRYPTION", Encryption)
                .Replace("@FLOW", Flow)
                .Replace("@SNI", Sni)
                .Replace("@PUBLIC_KEY_FILE", PublicKeyFile)
                .Replace("@PRIVATE_KEY_FILE", PrivateKeyFile);
        }
    }
}
