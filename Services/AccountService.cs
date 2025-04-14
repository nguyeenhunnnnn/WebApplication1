using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using WebApplication1.Models.ViewModels;
namespace WebApplication1.Services
{
    public interface IAccountService
    {
        Task<TaiKhoan> GetTaiKhoanByIdAsync(string id);
        Task<List<TaiKhoan>> GetAllTaiKhoansAsync();
        Task<bool> CreateTaiKhoanAsync(RegisterStep2ViewModel model);
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
        Task<bool> login(LoginViewModel model);
    }
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        public AccountService(IAccountRepository accountRepository, UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager)
        {
            _accountRepository = accountRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<TaiKhoan> GetTaiKhoanByIdAsync(string id)
        {
            return await _accountRepository.GetTaiKhoanByIdAsync(id);
        }
        public async Task<List<TaiKhoan>> GetAllTaiKhoansAsync()
        {
            return await _accountRepository.GetAllTaiKhoansAsync();
        }
        public async Task<bool> CreateTaiKhoanAsync(RegisterStep2ViewModel model)
        {
            var taiKhoan = new TaiKhoan
            {
                UserName = model.HoTen,
                Email = model.Email,
                PhoneNumber = model.SDT,
                DiaChi = model.DiaChi,
                CCCD = model.CCCD,
                
                FileAvata = model.sFile_Avata_Path,
                FileCCCD = model.sFile_CCCD_Path,
                VaiTro = model.VaiTro,
                TrangThai= "Hoạt động"
            };
            try
            {
                return await _accountRepository.CreateTaiKhoanAsync(taiKhoan,model.MatKhau);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
               return false;


            }
        }
        public async Task<bool> login(LoginViewModel model)
        {
            
            var taiKhoan = await _userManager.FindByEmailAsync(model.Email);
            if (taiKhoan != null)
            {
                var result = await _signInManager.PasswordSignInAsync(taiKhoan.UserName, model.MatKhau, true, lockoutOnFailure: true);
                return result.Succeeded;
            }
            return false;
        }
        public async Task<bool> UpdateTaiKhoanAsync(TaiKhoan taiKhoan)
        {
            return await _accountRepository.UpdateTaiKhoanAsync(taiKhoan);
        }
        public async Task<bool> DeleteTaiKhoanAsync(string id)
        {
            return await _accountRepository.DeleteTaiKhoanAsync(id);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _accountRepository.SaveChangesAsync();
        }
        public async Task<bool> DeleteChangesAsync()
        {
            return await _accountRepository.DeleteChangesAsync();
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _accountRepository.EmailExistsAsync(email);
        }
        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _accountRepository.PhoneExistsAsync(phone);
        }
        public async Task<bool> CCCDExistAsync(string CCCD)
        {
            return await _accountRepository.CCCDExistAsync(CCCD);
        }
        public async Task<bool> CheckTaiKhoanExistsAsync(string id)
        {
            return await _accountRepository.CheckTaiKhoanExistsAsync(id);
        }
        public async Task<bool> CheckTaiKhoanExistsByEmailAsync(string email)
        {
            return await _accountRepository.CheckTaiKhoanExistsByEmailAsync(email);
        }
        public async Task<bool> CheckTaiKhoanExistsByPhoneAsync(string phone)
        {
            return await _accountRepository.CheckTaiKhoanExistsByPhoneAsync(phone);
        }
      

    }
}
