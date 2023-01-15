namespace Xray.Hope.Web.Server.Models.Configurations
{
    public class HopeRateLimiterOptions
    {
        public int PermitLimit { get; set; }

        public int QueueLimit { get; set; }

        public double Window { get; set; }

        public int SegmentsPerWindow { get; set; }
    }
}
