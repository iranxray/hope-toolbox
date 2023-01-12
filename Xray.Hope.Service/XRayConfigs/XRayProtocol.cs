namespace Xray.Hope.Service.XRayConfigs
{
    public class XRayProtocol
    {
        public string Name { get; private set; }

        public readonly static XRayProtocol Trojan = new XRayProtocol
        {
            Name = "trojan"
        };

        public readonly static XRayProtocol VLESS = new XRayProtocol
        {
            Name = "vless"
        };

        public static implicit operator string(XRayProtocol protocol) => protocol?.Name;
    }
}
