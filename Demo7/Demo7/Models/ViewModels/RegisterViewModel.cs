using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Demo7.Models.ViewModels
{
    public class RegisterViewModel
    {
        [DisplayName("Tên đăng nhập")]
        [Required(ErrorMessage = "Tên đăng nhập không được trống")]
        [StringLength(20, MinimumLength = 3,
            ErrorMessage = "Tên đăng nhập phải có độ dài từ 3-20 ký tự")]
        public string UserName { get; set; }

        [DisplayName("Họ và tên")]
        [Required(ErrorMessage = "Họ và tên không được trống")]
        public string FullName { get; set; }

        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage = "Hãy nhập Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Gõ lại mật khẩu")]
        [Required(ErrorMessage = "Hãy nhập lại Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Hộp thư")]
        [Required(ErrorMessage = "Email không bỏ trống")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("Điện thoại")]
        [RegularExpression(@"^0\d{9,12}$",
            ErrorMessage = "Phải bắt đầu bằng 0 và dài 10-13 số")]
        public string Phone { get; set; }

        [DisplayName("Ngày sinh")]
        public DateTime Birthday { get; set; }
    }
}