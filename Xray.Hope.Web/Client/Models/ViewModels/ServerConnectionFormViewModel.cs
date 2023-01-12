using System.ComponentModel.DataAnnotations;

namespace Xray.Hope.Web.Client.Models.ViewModels
{
    public class ServerConnectionFormViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "باید آدرس سرور را وارد کنید.")]
        public string ServerAddress { get; set; }

        public int ServerSshPort { get; set; } = 22;

        public string ServerUsername { get; set; }

        public string ServerPassword { get; set; }

        public string PrivateKeyPath { get; set; }

        public string PrivateKeyContent { get; set; }

        [Range(2000, 60000, ErrorMessage = "باید پورت x-ui را وارد کنید. این عدد باید بین 20000 تا 60000 باشد.")]
        public int XuiPort { get; set; } = 34567;

        [Required(AllowEmptyStrings = false, ErrorMessage = "باید نام کاربری x-ui را وارد کنید.")]
        public string XuiUsername { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "باید رمز عبور x-ui را وارد کنید.")]
        public string XuiPassword { get; set; }
    }
}
