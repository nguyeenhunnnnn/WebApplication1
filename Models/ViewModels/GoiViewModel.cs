using WebApplication1.Models.Entities;
using WebApplication1.Models;

namespace WebApplication1.Models.ViewModels
{
    public class GoiViewModel
    {
        public int BaiDangId { get; set; }
        public List<GoiDichVu> GoiDichVus { get; set; }
    }
}
