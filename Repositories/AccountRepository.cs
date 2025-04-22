using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
namespace WebApplication1.Repositories
{
    public interface IAccountRepository
    {
        Task<TaiKhoan> GetTaiKhoanByIdAsync(string id);
        Task<List<TaiKhoan>> GetAllTaiKhoansAsync();
        Task<bool> CreateTaiKhoanAsync(TaiKhoan taiKhoan,string mk);
        Task<bool> UpdateTaiKhoanAsync(TaiKhoan taiKhoan);
        Task<bool> DeleteTaiKhoanAsync(string id);
        Task<bool> SaveChangesAsync();
        Task<bool> DeleteChangesAsync();
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneExistsAsync(string phone);
        Task<bool> CCCDExistAsync(string CCCD);
        Task<bool> CheckTaiKhoanExistsAsync(string id);
        Task<bool> CheckTaiKhoanExistsByEmailAsync(string email);
        Task<bool> CheckTaiKhoanExistsByPhoneAsync(string phone);
        Task<bool> login(TaiKhoan taiKhoan, string pass);
    }
    public class AccountRepository : IAccountRepository
    {
        public AccountRepository() { }
        private readonly ApplicationDbContext _context;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        public AccountRepository(ApplicationDbContext context, UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<TaiKhoan> GetTaiKhoanByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<List<TaiKhoan>> GetAllTaiKhoansAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<bool> CreateTaiKhoanAsync(TaiKhoan taiKhoan,string mk)
        {
            var result = await _userManager.CreateAsync(taiKhoan,mk);
            return result.Succeeded;
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Code: {error.Code}, Description: {error.Description}");
            }
        }
        public async Task<bool> login(TaiKhoan taiKhoan, string pass)
        {
            var result= await _signInManager.PasswordSignInAsync(taiKhoan.Email, pass, true, lockoutOnFailure: true);
            return result.Succeeded;
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.Users.AnyAsync(u => u.PhoneNumber == phone);
        }
        public async Task<bool> CCCDExistAsync(string CCCD)
        {
            return await _context.Users.AnyAsync(u => u.CCCD == CCCD);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateTaiKhoanAsync(TaiKhoan taiKhoan)
        {
            var existingTaiKhoan = await _context.Users.FindAsync(taiKhoan.Id);
            if (existingTaiKhoan == null)
            {
                return false;
            }
            existingTaiKhoan.UserName = taiKhoan.UserName;
            existingTaiKhoan.Email = taiKhoan.Email;
            existingTaiKhoan.DiaChi = taiKhoan.DiaChi;
            existingTaiKhoan.FileAvata = taiKhoan.FileAvata;
            existingTaiKhoan.FileCCCD = taiKhoan.FileCCCD;
            existingTaiKhoan.VaiTro = taiKhoan.VaiTro;
            existingTaiKhoan.TrangThai = taiKhoan.TrangThai;
            existingTaiKhoan.CCCD = taiKhoan.CCCD;
            _context.Users.Update(existingTaiKhoan);
            return await SaveChangesAsync();
        }
        public async Task<bool> DeleteTaiKhoanAsync(string id)
        {
            var taiKhoan = await _context.Users.FindAsync(id);
            if (taiKhoan == null)
            {
                return false;
            }
            _context.Users.Remove(taiKhoan);
            return await SaveChangesAsync();
        }
        public async Task<bool> DeleteChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> CheckTaiKhoanExistsAsync(string id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }
        public async Task<bool> CheckTaiKhoanExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        public async Task<bool> CheckTaiKhoanExistsByPhoneAsync(string phone)
        {
            return await _context.Users.AnyAsync(u => u.PhoneNumber == phone);
        }

    }
}
