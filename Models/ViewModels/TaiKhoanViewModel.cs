namespace WebApplication1.Models.ViewModels
{
    public class TaiKhoanViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string VaiTro { get; set; } // Vai trò của tài khoản (người dùng, nhà tuyển dụng, quản trị viên)
        public string DiaChi { get; set; } // Địa chỉ của tài khoản
        public string CCCD { get; set; } // Số chứng minh nhân dân hoặc căn cước công dân
        public string sFile_Avata_Path { get; set; } // Đường dẫn đến ảnh đại diện
        public string BaiDangTieuDe { get; set; }
        public string BaiDangId { get; set; } // ID của bài đăng liên quan đến tài khoản này

    }
}
