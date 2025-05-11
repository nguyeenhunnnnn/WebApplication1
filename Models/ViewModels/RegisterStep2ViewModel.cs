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

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập SĐT")]
        [StringLength(12, MinimumLength = 10, ErrorMessage = "SĐT phải từ 6 đến 100 ký tự")]
        //[RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập CCCD")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "CCCD phải gồm đúng 12 chữ số")]
        public string CCCD { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }
        public IFormFile sFile_Avata { get; set; }
        public IFormFile sFile_CCCD { get; set; }
        public string sFile_CCCD_Path { get; set; }
        public string sFile_Avata_Path { get; set; }
        public string? id { get; set; }

    }
}
