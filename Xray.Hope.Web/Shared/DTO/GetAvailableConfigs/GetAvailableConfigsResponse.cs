namespace Xray.Hope.Web.Shared.DTO.GetAvailableConfigs
{
    public class GetAvailableConfigsResponse
    {
        public IEnumerable<XrayConfig> TrojanConfigs { get; set; }

        public IEnumerable<XrayConfig> VlessConfigs { get; set; }
    }

    public class XrayConfig
    {
        public string Protocol { get; set; }

        public string Id { get; set; }

        public string Remark { get; set; }

        public string Encryption { get; set; }

        public string Network { get; set; }

        public int Port { get; set; }

        public bool Recommended { get; set; }

        public string Description { get; set; }

        public string GetDescription() => string.IsNullOrEmpty(Description)
            ? "نصب این کانفیگ نیاز به پیشنیازی ندارد. بعد از نصب از باز بودن پورت مطمئن شوید."
            : Description;
    }
}