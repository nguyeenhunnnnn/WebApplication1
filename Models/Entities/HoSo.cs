namespace WebApplication1.Models.Entities
{
    public class HoSo
    {
        public int iMaHS { get; set; } // Primary Key

        public string FK_iMaTK { get; set; } // Foreign Key đến TaiKhoan
        public TaiKhoan TaiKhoan { get; set; } = null!; // Navigation property

        public string sTieuDe { get; set; } = null!;
        //public int iKinhNghiem { get; set; }
        public string sBangCap { get; set; } = null!;
        public string sKyNang { get; set; } = null!;
        public string? sDuongDanTep { get; set; } // Tùy chọn (nullable)

        // Quan hệ với BaiDang (1-n, mỗi hồ sơ có thể có nhiều bài đăng)
        public List<BaiDang> BaiDangs { get; set; } = new List<BaiDang>();

        public string sKinhNghiem { get; set; }
    }
}
