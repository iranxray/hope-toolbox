using System.Text.Json.Serialization;

namespace Xray.Hope.Service.Xui
{
    /// <summary>
    /// X-UI responses are returned in this format.
    /// </summary>
    public class XuiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("msg")]
        public string Message { get; set; }

        public XuiErrorType? ErrorType
        {
            get
            {
                if (Message.Contains("添加失败: 端口已存在"))
                {
                    return XuiErrorType.PortExists;
                }

                return null;
            }
        }
    }
}
