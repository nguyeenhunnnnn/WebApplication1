namespace WebApplication1.Models.ViewModels
{
    public class LichSuTTViewModel
    {
        public int Id { get; set; }
        public string TenGoi { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public decimal SoTien { get; set; }
        //public string Loai { get; set; } // "Bài đăng" hoặc "Hồ sơ"
        //public int? BaiDangId { get; set; }
        // public int? HoSoId { get; set; }
        public string TenNguoiDung { get; set; }
        public string TieuDeBaiDang { get; set; }
        public DateTime NgayDang { get; set; }
        public DateTime? DayUuTienDen { get; set; }
        public string TrangThaiGD { get; set; }
    }
}
