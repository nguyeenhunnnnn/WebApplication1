using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Models;
using WebApplication1.Repositories;
using WebApplication1.Models.ViewModels;
using System.IO;
using System.Text;
namespace WebApplication1.Services
{   
    public interface IBangTinService
    {
        // Define methods for BangTin service
        // For example:
        // Task<IEnumerable<BangTin>> GetAllBangTinAsync();
        // Task<BangTin> GetBangTinByIdAsync(int id);
        // Task AddBangTinAsync(BangTin bangTin);
        // Task UpdateBangTinAsync(BangTin bangTin);
        // Task DeleteBangTinAsync(int id);
        Task<bool> SaveChangesAsync();
        Task<BaiDangPageViewModel> GetAllBaiDangsAsync(string currentUserId);
    }
    public class BangTinService : IBangTinService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBangTinRepository _bangTinRepository;
        private readonly UserManager<TaiKhoan> _userManager;

        public BangTinService(ApplicationDbContext context, IBangTinRepository bangTinRepository, UserManager<TaiKhoan> userManager)
        {
            _context = context;
            _bangTinRepository = bangTinRepository;
            _userManager = userManager;
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<BaiDangPageViewModel> GetAllBaiDangsAsync(string currentUserId)
        {
            var baiDangs = await _bangTinRepository.GetAllAsync();
            var currentUser = await _bangTinRepository.GetByIdAsync(currentUserId);

            var dsBaiDangViewModel = baiDangs.Select(b => new BaiDangViewModel
            {
                PK_iMaBaiDang = b.PK_iMaBaiDang,
                sTieuDe = b.sTieuDe,
                sMoTa = b.sMoTa,
                sDiaDiem = b.sDiaDiem,
                fMucLuong = b.fMucLuong ?? 0,
                sTrangThai = b.sTrangThai,
                dNgayTao = b.dNgayTao,
                dThoiGianHetHan = b.dThoiGianHetHan ?? DateTime.MinValue,
                Nguoitao = b.TaiKhoan.UserName,
                sfileAvata = b.TaiKhoan.FileAvata,
                sMonday = b.sMonday,
                sYCau = b.sYCau,
                sGioiTinh = b.sGioiTinh,
                sTuoi = b.sTuoi,
                sKinhNghiem = b.sKinhNghiem,
                Vaitro=b.TaiKhoan.VaiTro,
                FileCVPath = b.FileCVPath,
                sBangCap = b.sBangCap
            }).ToList();

            var thongTinNguoiDungVm = new ThongTinNguoiDungViewModel
            {
                HoTen = currentUser.UserName,
                Email = currentUser.Email,
                VaiTroND = currentUser.VaiTro,
                AvatarUrl = currentUser.FileAvata
            };

            return new BaiDangPageViewModel
            {
                ThongTinNguoiDung = thongTinNguoiDungVm,
                DanhSachBaiDang = dsBaiDangViewModel
            };
        }
    }
}
