using System;
using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;
using WebApplication1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Security.Claims;
using System.IO;
using System.Text;

namespace WebApplication1.Repositories
{
    public interface IBangTinRepository
    {
        // Define methods for BangTin repository
        // For example:
        // Task<IEnumerable<BangTin>> GetAllBangTinAsync();
        // Task<BangTin> GetBangTinByIdAsync(int id);
        // Task AddBangTinAsync(BangTin bangTin);
        // Task UpdateBangTinAsync(BangTin bangTin);
        // Task DeleteBangTinAsync(int id);
        Task<bool> SaveChangesAsync();
        Task<List<BaiDang>> GetAllAsync();
        Task<TaiKhoan> GetByIdAsync(string userId);
    }
    public class BangTinRepository : IBangTinRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<TaiKhoan> _userManager;

        public BangTinRepository(ApplicationDbContext context, UserManager<TaiKhoan> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<BaiDang>> GetAllAsync()
        {
            return await _context.BaiDangs
                .Include(b => b.TaiKhoan)
                 .Where(b => b.IsHidden == false)
                .OrderByDescending(b => b.dNgayTao)
                .ToListAsync();
        }
        public async Task<TaiKhoan> GetByIdAsync(string userId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

    }
}
