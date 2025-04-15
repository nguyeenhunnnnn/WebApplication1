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

namespace WebApplication1.Services
{
    public interface IDangTinService
    {
        // Define methods for DangTin service
        // For example:
        // Task<IEnumerable<DangTin>> GetAllDangTinAsync();
        // Task<DangTin> GetDangTinByIdAsync(int id);
        // Task AddDangTinAsync(DangTin dangTin);
        // Task UpdateDangTinAsync(DangTin dangTin);
        // Task DeleteDangTinAsync(int id);
        Task<bool> SaveChangesAsync();
        Task<bool> CreatDangTin(DangTinViewModel model, string userId);
        Task<BaiDang> GetDangTinById(int id);
        Task<bool> UpdateDangTin(BaiDang dangTin);
        Task<bool> DeleteDangTin(int id);
        Task<bool> DangTinExistsAsync(int id);
        Task<bool> DangTinExistsByTitleAsync(string title);
        Task<List<BaiDangViewModel>> GetAllBaiDangByTaiKhoanId(string taiKhoanId,string trangthai);

    }
    public class DangTinService : IDangTinService
    {
      public  DangTinService() { }
        private readonly ApplicationDbContext _context;
        private readonly IDangTinRepository _dangTinRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        public DangTinService(ApplicationDbContext context, IDangTinRepository dangTinRepository, IAccountRepository accountRepository, UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager)
        {
            _context = context;
            _dangTinRepository = dangTinRepository;
            _accountRepository = accountRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<bool> CreatDangTin(DangTinViewModel model, string userId)
        {
            var dangTin = new BaiDang(); 
            dangTin.sMonday = model.sMonday;
            dangTin.sYCau = model.sYCau;
            dangTin.sGioiTinh = model.sGioiTinh;
            dangTin.sTuoi = model.sTuoi;
            dangTin.sKinhNghiem = model.sKinhNghiem;
            dangTin.sBangCap = model.sBangCap;
            dangTin.sTieuDe = model.sTieuDe;
            dangTin.sDiaDiem = model.sDiaDiem;
            dangTin.fMucLuong = model.fMucLuong;
            dangTin.dThoiGianHetHan = model.dThoiGianHetHan;
            dangTin.sMoTa = model.sMoTa;
            dangTin.dNgayTao = DateTime.Now;
            dangTin.sTrangThai = "Đang chờ duyệt";
            dangTin.FK_iMaTK = userId;
            try
            {
                await _dangTinRepository.CreatDangTin(dangTin);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Log the exception or handle it as needed
                return false;
            }
            
        }
        public async Task<BaiDang> GetDangTinById(int id)
        {
            return await _context.BaiDangs.FindAsync(id);
        }
        public async Task<bool> UpdateDangTin(BaiDang dangTin)
        {
            _context.BaiDangs.Update(dangTin);
            return await SaveChangesAsync();
        }
        public async Task<bool> DeleteDangTin(int id)
        {
            var dangTin = await GetDangTinById(id);
            if (dangTin != null)
            {
                _context.BaiDangs.Remove(dangTin);
                return await SaveChangesAsync();
            }
            return false;
        }
        public async Task<bool> DangTinExistsAsync(int id)
        {
            return await _context.BaiDangs.AnyAsync(d => d.PK_iMaBaiDang == id);
        }
        public async Task<bool> DangTinExistsByTitleAsync(string title)
        {
            return await _dangTinRepository.DangTinExistsByTitleAsync(title);
        }
        
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DangTinExistsById(int id)
        {
            return await _context.BaiDangs.AnyAsync(d => d.PK_iMaBaiDang == id);
        }
        public async Task<bool> DangTinExistsByTitle(string title)
        {
            return await _context.BaiDangs.AnyAsync(d => d.sTieuDe == title);
        }
        public async Task<bool> DangTinExistsByLocation(string location)
        {
            return await _context.BaiDangs.AnyAsync(d => d.sDiaDiem == location);
        }
        public async Task<bool> DangTinExistsByIdAsync(int id)
        {
            return await _context.BaiDangs.AnyAsync(d => d.PK_iMaBaiDang == id);
        }
        public async Task<List<BaiDangViewModel>> GetAllBaiDangByTaiKhoanId(string taiKhoanId,string trangthai)
        {
            var baiDangs = await _dangTinRepository.GetAllBaiDangByTaiKhoanId(taiKhoanId,trangthai);

           return baiDangs.Select(b=>new BaiDangViewModel
                {
                    sMonday =b.sMonday,
                    sBangCap=b.sBangCap,
                    sGioiTinh=b.sGioiTinh,
                    sKinhNghiem=b.sKinhNghiem,
                    sTuoi=b.sTuoi,
                    sYCau=b.sYCau,
                    sTieuDe = b.sTieuDe,
                    sMoTa = b.sMoTa,
                    sDiaDiem = b.sDiaDiem,
                    fMucLuong = b.fMucLuong ?? 0,
                    sTrangThai = b.sTrangThai,
                    dNgayTao = b.dNgayTao,
                    dThoiGianHetHan = b.dThoiGianHetHan ?? DateTime.MinValue
                })
                .ToList();
          
        }
    }
}
