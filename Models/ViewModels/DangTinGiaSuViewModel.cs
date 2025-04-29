namespace WebApplication1.Models.ViewModels
{
    public class DangTinGiaSuViewModel
    {
        public string sTieuDe { get; set; } = null!;
        public string sMoTa { get; set; } = null!;
        public string sDiaDiem { get; set; }
        public decimal fMucLuong { get; set; }
        public DateTime? dThoiGianHetHan { get; set; }
        public string sMonday { get; set; }
        public string sYCau { get; set; }
        public string sGioiTinh { get; set; }
        public string sTuoi { get; set; }
        public string sKinhNghiem { get; set; }
        public string sBangCap { get; set; }
        // File CV được lấy tự động từ Hồ sơ
        public string? FileCVPath { get; set; }  // Dành cho việc hiển thị CV từ hồ sơ
    }

}
