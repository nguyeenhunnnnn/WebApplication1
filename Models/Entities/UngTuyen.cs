namespace WebApplication1.Models.Entities
{
    public class UngTuyen
    {
        public int Id { get; set; }

        public string FK_iMaTK_GiaSu { get; set; }
        public TaiKhoan TaiKhoanGiaSu { get; set; }

        public int FK_iMaBaiDang { get; set; }
        public BaiDang BaiDang { get; set; }

        public int FK_iMaHS { get; set; }
        public HoSo HoSo { get; set; }

        public string TrangThai { get; set; } 

        public DateTime NgayUngTuyen { get; set; } = DateTime.Now;
    }
}
