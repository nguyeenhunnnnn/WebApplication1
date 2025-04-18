﻿namespace WebApplication1.Models.ViewModels
{
    public class HoSoViewModel
    {
        public int iMaHS { get; set; } // Primary Key
        public string FK_iMaTK { get; set; } // Foreign Key đến TaiKhoan
        public string HoTen { get; set; } 
        public string SoDienThoai { get; set; } 
        public string Email { get; set; }
        public string DiaChi { get; set; } 
        public string sTieuDe { get; set; } 
        public string sBangCap { get; set; }
        public string sKinhNghiem { get; set; }
        public string sKyNang { get; set; } 
        public string sDuongDanTep { get; set; } // Tùy chọn (nullable)
        public IFormFile formFile { get; set; }
        public string anhDaiDien { get; set; } // Đường dẫn đến ảnh đại diện
        // Quan hệ với BaiDang (1-n, mỗi hồ sơ có thể có nhiều bài đăng)
        public List<BaiDangViewModel> BaiDangs { get; set; } = new List<BaiDangViewModel>();
        public string sTrangThai { get; set; }
        public IFormFile formAnhBC { get; set; }
        public string sDuongDanTepBC { get; set; } // Đường dẫn đến ảnh bằng cấp
    }
}
