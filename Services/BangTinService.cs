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
        private readonly IThanhToanRepository _thanhToanRepo;
        private readonly UserManager<TaiKhoan> _userManager;

        public BangTinService(ApplicationDbContext context, IBangTinRepository bangTinRepository, UserManager<TaiKhoan> userManager, IThanhToanRepository thanhToanRepo)
        {
            _context = context;
            _bangTinRepository = bangTinRepository;
            _userManager = userManager;
            _thanhToanRepo = thanhToanRepo;
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<BaiDangPageViewModel> GetAllBaiDangsAsync(string currentUserId)
        {
            var baiDangs = await _bangTinRepository.GetAllAsync();
            TaiKhoan currentUser = null;

            if (!string.IsNullOrEmpty(currentUserId))
            {
                currentUser = await _bangTinRepository.GetByIdAsync(currentUserId);
            }

            var dsBaiDangViewModel = baiDangs
                .OrderByDescending(b => b.dUuTienDen > DateTime.Now ? b.dUuTienDen : b.dNgayTao)
                .Select(b => new BaiDangViewModel
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
                
                sBangCap = b.sBangCap,
                    // cot trang thai giao dich thnah toan
                    sTrangThaiGD = b.sTrangThaiGD
                }).ToList();
            var thongTinNguoiDungVm = new ThongTinNguoiDungViewModel();

            if (currentUser != null)
            {
                thongTinNguoiDungVm.HoTen = currentUser.UserName;
                thongTinNguoiDungVm.Email = currentUser.Email;
                thongTinNguoiDungVm.VaiTroND = currentUser.VaiTro;
                thongTinNguoiDungVm.mondaygiasu = currentUser.BaiDangs.Select(b => b.sMonday).FirstOrDefault() ?? "";
                thongTinNguoiDungVm.GoiCuoc = currentUser.GoiCuoc;
                thongTinNguoiDungVm.AvatarUrl = currentUser.FileAvata;

                thongTinNguoiDungVm.GoiDichVu = await _thanhToanRepo.GetGoiDichVuByUserIdAsync(currentUser.Id);
            }


            return new BaiDangPageViewModel
            {
                ThongTinNguoiDung = thongTinNguoiDungVm,
                DanhSachBaiDang = dsBaiDangViewModel
            };
        }
    }
}
