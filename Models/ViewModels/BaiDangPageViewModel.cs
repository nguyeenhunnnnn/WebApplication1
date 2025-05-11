using WebApplication1.Models.Entities;

namespace WebApplication1.Models.ViewModels
{
    public class BaiDangPageViewModel
    {
        public ThongTinNguoiDungViewModel ThongTinNguoiDung { get; set; }
        public List<BaiDangViewModel> DanhSachBaiDang { get; set; }
        
    }


    public class ThongTinNguoiDungViewModel
    {
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public string VaiTroND { get; set; } // Vai trò của người dùng (người tìm việc hoặc nhà tuyển dụng)
       // public string GoiCuoc { get; set; }
        public string? mondaygiasu { get; set; }
        public bool GoiCuoc { get; set; } = false;
        public GoiDichVu? GoiDichVu { get; set; } // Thêm dòng này

    }

       

    }

