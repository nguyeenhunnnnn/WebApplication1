using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModels
{
    public class RegisterStep2ViewModel
    {
        [Required]
        public string VaiTro { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; }
        public string CCCD { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }
        public IFormFile sFile_Avata { get; set; }
        public IFormFile sFile_CCCD { get; set; }
        public string sFile_CCCD_Path { get; set; }
        public string sFile_Avata_Path { get; set; }

    }
}
