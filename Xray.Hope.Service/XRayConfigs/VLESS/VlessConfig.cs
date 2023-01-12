using Microsoft.AspNetCore.WebUtilities;
using System.Net;

namespace Xray.Hope.Service.XRayConfigs.VLESS
{
    /// <summary>
    /// Represents VLESS inbound protocol.
    /// </summary>
    public record VlessConfig : XRayConfig
    {
        private static string Template = @"
                up= 0&
                down= 0&
                total= 0&
                remark= @REMARK&
                enable= true&
                expiryTime= 0&
                listen= &
                port= @PORT&
                protocol= vless&
                settings= 
                {
                  ""clients"": [
                    {
                      ""id"": ""@CLIENT_ID"",
                      ""flow"": ""@FLOW""
                    }
                  ],
                  ""decryption"": ""none"",
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
                  ""wsSettings"": {
                    ""path"": ""/"",
                    ""headers"": {
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

        public VlessConfig()
        {
            Protocol = XRayProtocol.VLESS;
        }

        public override XRayConfigLink Link
        {
            get
            {
                var configParams = new Dictionary<string, string>()
                {
                    { "security", Encryption },
                    { "host", Sni },
                    { "sni", Sni },
                    { "flow", Flow },
                    { "type", Network},
                };

                var uri = QueryHelpers
                    .AddQueryString($"vless://{ClientId}@{Host}:{Port}", configParams)
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
                    ? string.Empty
                    : Headers.Select(kv => $"\"{kv.Key}\": \"{kv.Value}\"").Aggregate((a, b) => a + Environment.NewLine + b))
                .Replace(Environment.NewLine, string.Empty)
                .Replace("@CLIENT_ID", ClientId)
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
