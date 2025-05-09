using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Entities
{
    public class GoiDichVu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TenGoi { get; set; }

        
        public string MoTa { get; set; }

        
        public decimal Gia { get; set; }

        [Required]
        public int SoNgayHieuLuc { get; set; }

        public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();

    }
}
