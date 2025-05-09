using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Entities
{
    public class ThanhToan
    {
        [Key]
        public int Id { get; set; }

        // lien ket voi tai khoan
        [Required]
        public String TaiKhoanId { get; set; }

        [ForeignKey("TaiKhoanId")]
        public TaiKhoan TaiKhoan { get; set; }
        // lien ket voi goi dich vu
        [Required]
        public int GoiDichVuId { get; set; }

        [ForeignKey("GoiDichVuId")]
        public GoiDichVu GoiDichVu { get; set; }

        [Required]
        public DateTime NgayThanhToan { get; set; }
        // lien ket voi bai dang
        public int? BaiDangId { get; set; } // FK đến Bài đăng phụ huynh (nullable)

        [ForeignKey("BaiDangId")]
        public BaiDang? BaiDang { get; set; }
        // trang thái thanh toán đã được duyet hay chưa 
        public bool IsDuyet { get; set; } = false;
    }
}

