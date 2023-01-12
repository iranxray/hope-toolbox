using Xray.Hope.Web.Client.Models.Domain.Constants;

namespace Xray.Hope.Web.Client.Models.Domain
{

    public class ToastMessage
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public NotificationStatus Status { get; set; }

        /// <summary>
        /// Delay in milliseconds.
        /// </summary>
        public int Delay { get; set; } = 5000;
    }
}
