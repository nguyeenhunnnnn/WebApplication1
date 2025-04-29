namespace WebApplication1.Models.Entities
{
    public class DanhGiaGiaSu
    {
        public int Id { get; set; }

        public string NguoiDanhGiaId { get; set; } = null!; // Phụ huynh
        public TaiKhoan NguoiDanhGia { get; set; } = null!;

        public string GiaSuId { get; set; } = null!;         // Gia sư
        public TaiKhoan GiaSu { get; set; } = null!;

        public int SoSao { get; set; }
        public string? NoiDung { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
