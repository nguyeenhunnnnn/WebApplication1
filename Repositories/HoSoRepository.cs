using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Repositories
{
    public interface IHoSoRepository
    {
        Task<bool> SaveChangesAsync();
        Task<bool> CreateHoSoAsync(HoSo hoSo);
        Task<bool> DeleteChangesAsync(int id);
        //  Task<bool> UpdateChangesAsync();
        Task<List<HoSo>> GetAllHoSoByTaiKhoanId(string taiKhoanId, string trangthai);
        
        Task<HoSo> GetHoSoById(int id);
        Task<List<HoSo>> GetAllHoSoByTrangThai(string trangthai);
        Task<bool> UpdateTrangThaiHS(int id, string trangThai);


    }
    public class HoSoRepository : IHoSoRepository
    {
        private readonly ApplicationDbContext _context;

        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        public HoSoRepository(ApplicationDbContext context, IAccountRepository accountRepository, UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager)
        {
            _context = context;

            _accountRepository = accountRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> CreateHoSoAsync(HoSo hoSo)
        {
            await _context.HoSos.AddAsync(hoSo);
            return await SaveChangesAsync();
        }
        public async Task<List<HoSo>> GetAllHoSoByTaiKhoanId(string taiKhoanId, string trangthai)
        {
            var query = _context.HoSos
        .Where(h => h.sTrangThai == trangthai && h.FK_iMaTK == taiKhoanId)
        .Include(h => h.TaiKhoan);
            // Lọc theo TaiKhoanId
            return await query.ToListAsync();
        }
        
        public async Task<HoSo> GetHoSoById(int id)
        {
            return await _context.HoSos
                .Include(hs => hs.TaiKhoan)
                .FirstOrDefaultAsync(hs => hs.iMaHS == id);
        }
        public async Task<bool> DeleteChangesAsync(int id)
        {
            var result=await GetHoSoById(id);
            if(result==null)
            { return  false; }
            _context.HoSos.Remove(result);
            return await SaveChangesAsync();
        }
        public async Task<List<HoSo>> GetAllHoSoByTrangThai(string trangthai)
        {
            var query = _context.HoSos
            .Where(b => b.sTrangThai == trangthai)
            .Include(h => h.TaiKhoan);
            // Lọc theo TaiKhoanId
            return await query.ToListAsync();
        }
        public async Task<bool> UpdateTrangThaiHS(int id, string trangThai)
        {
            var hs = await _context.HoSos.FindAsync(id);
            if (hs == null) return false;

            hs.sTrangThai = trangThai;
            return await SaveChangesAsync();
        }

    }
}
