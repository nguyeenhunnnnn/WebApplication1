using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Entities;

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

        // lien ket voi thanh toan hien thi goi
        public bool GoiCuoc { get; set; } = false;
        
        public string? TieuDeCV { get; set; }
        

        //danh gia
        public List<DanhGiaGiaSu> DanhGias { get; set; } = new();
        public double DiemTrungBinh { get; set; }
        public int TongDanhGia => DanhGias.Count;

        public int SoSao1 => DanhGias.Count(d => d.SoSao == 1);
        public int SoSao2 => DanhGias.Count(d => d.SoSao == 2);
        public int SoSao3 => DanhGias.Count(d => d.SoSao == 3);
        public int SoSao4 => DanhGias.Count(d => d.SoSao == 4);
        public int SoSao5 => DanhGias.Count(d => d.SoSao == 5);
    }
}
