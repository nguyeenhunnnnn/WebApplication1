using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
namespace WebApplication1.Repositories
{
    public interface IDangTinRepository
    {
        // Define methods for DangTin repository
        // For example:
        // Task<IEnumerable<DangTin>> GetAllDangTinAsync();
        // Task<DangTin> GetDangTinByIdAsync(int id);
        // Task AddDangTinAsync(DangTin dangTin);
        // Task UpdateDangTinAsync(DangTin dangTin);
        // Task DeleteDangTinAsync(int id);
        Task<bool> SaveChangesAsync();
        Task<bool> CreatDangTin(BaiDang dangTin);
        Task<BaiDang> GetDangTinById(int id);
        Task<bool> UpdateDangTin(BaiDang dangTin);
        Task<bool> DeleteDangTin(int id);
        Task<bool> DangTinExistsAsync(int id);
        Task<bool> DangTinExistsByTitleAsync(string title);
        Task<List<BaiDang>> GetAllBaiDangByTaiKhoanId(string taiKhoanId,string trangthai);



    }
    public class DangTinRepository : IDangTinRepository
    {
      public  DangTinRepository() { }
        private readonly ApplicationDbContext _context;
    
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        public DangTinRepository(ApplicationDbContext context, IAccountRepository accountRepository, UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager)
        {
            _context = context;
           
            _accountRepository = accountRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public DangTinRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreatDangTin(BaiDang dangTin)
        {
            await _context.BaiDangs.AddAsync(dangTin);
            return await SaveChangesAsync();
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
            if (dangTin == null) return false;
            _context.BaiDangs.Remove(dangTin);
            return await SaveChangesAsync();
        }
        public async Task<bool> DangTinExistsAsync(int id)
        {
            return await _context.BaiDangs.AnyAsync(e => e.PK_iMaBaiDang == id);
        }
        public async Task<bool> DangTinExistsByTitleAsync(string title)
        {
            return await _context.BaiDangs.AnyAsync(e => e.sTieuDe == title);
        }
      
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<BaiDang>> GetAllBaiDangByTaiKhoanId(string taiKhoanId,string trangthai)
        {
            var query = _context.BaiDangs
            .Where(b => b.FK_iMaTK == taiKhoanId); // Lọc theo TaiKhoanId

            if (!string.IsNullOrEmpty(trangthai)) // Nếu trạng thái được truyền vào, lọc thêm theo trạng thái
            {
                query = query.Where(b => b.sTrangThai == trangthai);
            }
            return await query.ToListAsync();
        }
    }
}
