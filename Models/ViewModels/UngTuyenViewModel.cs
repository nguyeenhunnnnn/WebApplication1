namespace WebApplication1.Models.ViewModels
{
    public class UngTuyenViewModel
    {
        public int Id { get; set; }
        public string TenGiaSu { get; set; }
        public string GiaSuID { get; set; }
        public string EmailGiaSu { get; set; }
        public string AvatarUrl { get; set; }
        public int IdHoSo { get; set; } // ID của hồ sơ ứng tuyển
        public string TieuDeHoSo { get; set; }
        public string BangCap { get; set; }
        public string phuhuynhName { get; set; } 
        public string PhuHuynhId { get; set; }// ID của phụ huynh
        public string TrangThai { get; set; }
        public DateTime NgayUngTuyen { get; set; }
        public int MaBaiDang { get; set; }
        public string TieuDeBaiDang { get; set; }
        public string fileCV { get; set; } // Đường dẫn đến file CV
    }
}
