using WebApplication1.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModels
{
    public class DangTinViewModel
    {
        public string FK_iMaTK { get; set; } // Bắt buộc
        public TaiKhoan TaiKhoan { get; set; } = null!;

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        
        [Display(Name = "Tiêu đề")]
        public string sTieuDe { get; set; } = null!;
        
        [Display(Name = "Mô tả")]
        public string? sMoTa { get; set; }

        public string? sDiaDiem { get; set; }
        public decimal? fMucLuong { get; set; }

        public string sTrangThai { get; set; } = null!;

        public DateTime dNgayTao { get; set; }
        public DateTime? dThoiGianHetHan { get; set; } // Tùy chọn

        public int? FK_iMaHS { get; set; } // Tùy chọn
        public HoSo? HoSo { get; set; }

        //cột mới 
        public string sMonday { get; set; }
        public string sYCau { get; set; }
        public string sGioiTinh { get; set; }
        public string sTuoi { get; set; }
        public string sKinhNghiem { get; set; }
        public string sBangCap { get; set; }
    }
}
