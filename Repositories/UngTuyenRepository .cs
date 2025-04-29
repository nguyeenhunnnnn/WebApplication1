using System;
using WebApplication1.Data;
using WebApplication1.Models.ViewModels;
using WebApplication1.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace WebApplication1.Repositories
{
    public interface IUngTuyenRepository
    {
        Task<List<UngTuyen>> LayUngVienTheoBaiDang(int maBaiDang);
        Task<List<UngTuyen>> LayUngTuyenCuaGiaSu(string maTaiKhoan);
        Task ThemUngTuyenAsync(UngTuyen ungTuyen);
        Task CapNhatTrangThaiAsync(int id, string trangThai);
        Task<List<UngTuyen>> LayDanhSachUngVienCuaPhuHuynh(string phuHuynhId, string trangthai);

    }
    public class UngTuyenRepository : IUngTuyenRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;

        public UngTuyenRepository(ApplicationDbContext context, IAccountRepository accountRepository,UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager)
        {
            _context = context;
            _accountRepository = accountRepository;
            _userManager = userManager;
            _signInManager = signInManager;

        }
        public async Task<List<UngTuyen>> LayUngVienTheoBaiDang(int maBaiDang) =>
        await _context.UngTuyen
            .Include(u => u.TaiKhoanGiaSu)
            .Include(u => u.HoSo)
            .Where(u => u.FK_iMaBaiDang == maBaiDang)
            .ToListAsync();

        public async Task<List<UngTuyen>> LayUngTuyenCuaGiaSu(string maTK)
        {
            return await _context.UngTuyen
                .Include(u => u.BaiDang)
                    .ThenInclude(bd => bd.TaiKhoan) // lấy thông tin phụ huynh
                .Include(u => u.HoSo)
                .Where(u => u.FK_iMaTK_GiaSu == maTK)
                .ToListAsync();
        }

        public async Task ThemUngTuyenAsync(UngTuyen ungTuyen)
        {
            _context.UngTuyen.Add(ungTuyen);
            await _context.SaveChangesAsync();
        }

        public async Task CapNhatTrangThaiAsync(int id, string trangThai)
        {
            var item = await _context.UngTuyen.FindAsync(id);
            if (item != null)
            {
                item.TrangThai = trangThai;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<UngTuyen>> LayDanhSachUngVienCuaPhuHuynh(string phuHuynhId, string trangthai)
        {
            return await _context.UngTuyen
                .Include(ut => ut.TaiKhoanGiaSu)
                .Include(ut => ut.BaiDang)
                    .ThenInclude(bd => bd.TaiKhoan) // để truy xuất tác giả bài đăng
                .Include(ut => ut.HoSo)
                .Where(ut => ut.BaiDang.FK_iMaTK == phuHuynhId && ut.TrangThai == trangthai)
                .ToListAsync();
        }
    }
}
