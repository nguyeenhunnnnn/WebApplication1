namespace WebApplication1.Models.Entities
{
    public class TinNhan
    {
        public int Id { get; set; }
        public string NguoiGuiId { get; set; }
        public string NguoiNhanId { get; set; }
        public string NoiDung { get; set; }
        public DateTime ThoiGianGui { get; set; }

        public int? BaiDangId { get; set; } // nullable
        public BaiDang BaiDang { get; set; }

        public TaiKhoan NguoiGui { get; set; }
        public TaiKhoan NguoiNhan { get; set; }
    }
}
