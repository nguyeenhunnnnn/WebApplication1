using WebApplication1.Models.Entities;

namespace WebApplication1.Models.ViewModels
{
    public class DanhGiaViewModel
    {
        public string GiaSuId { get; set; } = null!;
        public List<DanhGiaGiaSu> DanhGias { get; set; } = new();
        public double DiemTrungBinh { get; set; }
    }
}
