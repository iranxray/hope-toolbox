using Xray.Hope.Web.Client.Models.Domain;
using Xray.Hope.Web.Client.Models.Domain.Constants;

namespace Xray.Hope.Web.Client.Services
{
    public class NotificationService
    {
        public event Action<ToastMessage> OnNotify;

        public void AddMessage(
            string title,
            string body,
            NotificationStatus status = NotificationStatus.Success,
            int delayInMs = 5000)
        {
            var message = new ToastMessage
            {
                Title = title,
                Body = body,
                Status = status,
                Delay = delayInMs,
            };

            OnNotify?.Invoke(message);
        }
    }
}
