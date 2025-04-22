using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModels
{
    public class ProfileViewModel
    {
       
        public string VaiTro { get; set; }
      
        public string Email { get; set; }
       
        public string MatKhau { get; set; }

        
        public string HoTen { get; set; }

       
        public string SDT { get; set; }
        public string CCCD { get; set; }

        public string? DiaChi { get; set; }
        public IFormFile sFile_Avata { get; set; }
        public IFormFile sFile_CCCD { get; set; }
        public string sFile_CCCD_Path { get; set; }
        public string sFile_Avata_Path { get; set; }
    }
}
