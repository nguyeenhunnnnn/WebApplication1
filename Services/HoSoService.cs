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
    public interface IHoSoService
    {
        Task<bool> CreateHoSoAsync(HoSoViewModel hoSo,string id);
        Task<bool> DeleteChangesAsync(int id);
       // Task<bool> GetChangesAsync();
       // Task<bool> UpdateChangesAsync();
        Task<List<HoSoViewModel>> GetAllHoSoByTaiKhoanId(string taiKhoanId);
        Task<HoSoViewModel> GetHoSoById(int id);

    }
    public class HoSoService : IHoSoService
    {
        public HoSoService() { }
        private readonly ApplicationDbContext _context;
        private readonly IHoSoRepository _HoSoRepository;

        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        public HoSoService(ApplicationDbContext context, IHoSoRepository hoSoRepository, IAccountRepository accountRepository, UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager)
        {
            _context = context;
            _HoSoRepository = hoSoRepository;
            _accountRepository = accountRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<bool> CreateHoSoAsync(HoSoViewModel model,string id)
        {
            var hoSo = new HoSo
            {
                FK_iMaTK = id,
                sKinhNghiem = model.sKinhNghiem,
                sBangCap = model.sBangCap,
                sKyNang = model.sKyNang,
                sTieuDe = model.sTieuDe,
                sDuongDanTep = model.sDuongDanTep // Lưu đường dẫn tệp vào cơ sở dữ liệu
            };
            try
            {
                await _HoSoRepository.CreateHoSoAsync(hoSo);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Log the exception or handle it as needed
                return false;
            }

        }
        public async Task<List<HoSoViewModel>> GetAllHoSoByTaiKhoanId(string taiKhoanId)
        {
            var hoSoList=await _HoSoRepository.GetAllHoSoByTaiKhoanId(taiKhoanId);
            return hoSoList.Select(b => new HoSoViewModel
            {
                iMaHS = b.iMaHS,
                FK_iMaTK = b.FK_iMaTK,
                sKinhNghiem = b.sKinhNghiem,
                sBangCap = b.sBangCap,
                sKyNang = b.sKyNang,
                sTieuDe = b.sTieuDe,
                sDuongDanTep=b.sDuongDanTep,
                HoTen = b.TaiKhoan.UserName, // nếu có liên kết navigation property
                SoDienThoai = b.TaiKhoan.PhoneNumber,
                Email = b.TaiKhoan.Email,
                DiaChi = b.TaiKhoan.DiaChi
            }).ToList();

        }
        public async Task<HoSoViewModel> GetHoSoById(int id)
        {
            var hoSo=await _HoSoRepository.GetHoSoById(id);
            if(hoSo == null)
            { return null; }
            return new HoSoViewModel
            {
                iMaHS = hoSo.iMaHS,
                FK_iMaTK = hoSo.FK_iMaTK,
                sKinhNghiem = hoSo.sKinhNghiem,
                sBangCap = hoSo.sBangCap,
                sKyNang = hoSo.sKyNang,
                sTieuDe=hoSo.sTieuDe,
                sDuongDanTep=hoSo.sDuongDanTep,

                HoTen = hoSo.TaiKhoan.UserName, // nếu có liên kết navigation property
                SoDienThoai = hoSo.TaiKhoan.PhoneNumber,
                Email = hoSo.TaiKhoan.Email,
                DiaChi = hoSo.TaiKhoan.DiaChi
            };
        }
        public async Task<bool> DeleteChangesAsync(int id)
        {
            return await _HoSoRepository.DeleteChangesAsync(id);
        }
    }
}
