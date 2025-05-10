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
        Task<List<HoSoViewModel>> GetAllHoSoByTaiKhoanId(string taiKhoanId, string trangthai);
      
        Task<HoSoViewModel> GetHoSoById(int id);
        Task<List<HoSoViewModel>> GetAllHoSoByTrangThai(string trangthai);
        Task<bool> PheDuyetHSAsync(int id);
        Task<bool> UpdateHoSoAsync(HoSoViewModel model);
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
                sTrangThai = model.sTrangThai,
                sDuongDanTepBC = model.sDuongDanTepBC,
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
        public async Task<List<HoSoViewModel>> GetAllHoSoByTaiKhoanId(string taiKhoanId, string trangthai)
        {
            var hoSoList=await _HoSoRepository.GetAllHoSoByTaiKhoanId(taiKhoanId,trangthai);
            return hoSoList.Select(b => new HoSoViewModel
            {
                iMaHS = b.iMaHS,
                FK_iMaTK = b.FK_iMaTK,
                sKinhNghiem = b.sKinhNghiem,
                sBangCap = b.sBangCap,
                sKyNang = b.sKyNang,
                sTrangThai = b.sTrangThai,
                sTieuDe = b.sTieuDe,
                sDuongDanTep=b.sDuongDanTep,
                sDuongDanTepBC = b.sDuongDanTepBC,
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
                sTrangThai = hoSo.sTrangThai,
                anhDaiDien =hoSo.TaiKhoan.FileAvata,
                sDuongDanTepBC = hoSo.sDuongDanTepBC,
                sDuongDanTep =hoSo.sDuongDanTep,
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
        public async Task<List<HoSoViewModel>> GetAllHoSoByTrangThai(string trangthai)
        {
            var hoSoList = await _HoSoRepository.GetAllHoSoByTrangThai(trangthai);
            return hoSoList.Select(b => new HoSoViewModel
            {
                iMaHS = b.iMaHS,
                FK_iMaTK = b.FK_iMaTK,
                sKinhNghiem = b.sKinhNghiem,
                sBangCap = b.sBangCap,
                sKyNang = b.sKyNang,
                sTrangThai = b.sTrangThai,
                sTieuDe = b.sTieuDe,
                sDuongDanTep = b.sDuongDanTep,
                sDuongDanTepBC = b.sDuongDanTepBC,
                HoTen = b.TaiKhoan.UserName, // nếu có liên kết navigation property
                SoDienThoai = b.TaiKhoan.PhoneNumber,
                Email = b.TaiKhoan.Email,
                DiaChi = b.TaiKhoan.DiaChi
            }).ToList();
        }
        public async Task<bool> PheDuyetHSAsync(int id)
        {
            var baiDang = await _HoSoRepository.GetHoSoById(id);
            if (baiDang == null) return false;

            return await _HoSoRepository.UpdateTrangThaiHS(id, "Đã duyệt");
        }
        public async Task<bool> UpdateHoSoAsync(HoSoViewModel model)
        {
            var entity = await _HoSoRepository.GetByIdAsync(model.iMaHS);
            if (entity == null) return false;

            entity.sTieuDe = model.sTieuDe;
            entity.sKinhNghiem = model.sKinhNghiem;
            entity.sBangCap = model.sBangCap;
            entity.sKyNang = model.sKyNang;
            entity.sTrangThai = model.sTrangThai;
            entity.sDuongDanTep = model.sDuongDanTep;
            entity.sDuongDanTepBC = model.sDuongDanTepBC;
            await _HoSoRepository.UpdateAsync(entity);
            return true;
        }
    }
}
