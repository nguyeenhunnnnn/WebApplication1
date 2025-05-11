using System;
using WebApplication1.Data;
using WebApplication1.Models.ViewModels;
using WebApplication1.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Text;
using System.Collections.Generic;


namespace WebApplication1.Repositories
{
    public interface IUngTuyenRepository
    {
        Task<List<UngTuyen>> LayUngVienTheoBaiDang(int maBaiDang);
        Task<List<UngTuyen>> LayUngTuyenCuaGiaSu(string maTaiKhoan, string trangthai);
        Task ThemUngTuyenAsync(UngTuyen ungTuyen);
        Task CapNhatTrangThaiAsync(int id, string trangThai);
        Task<List<UngTuyen>> LayDanhSachUngVienCuaPhuHuynh(string phuHuynhId, string trangthai);
        Task<UngTuyen> GetUngTuyenByIdAsync(int id);
        Task<List<UngTuyen>> GetUngTuyenByBaiDangId(int baiDangId);
        Task<bool> CheckGiaSuHasAppliedAsync(string maTK, int maBaiDang);
        Task<UngTuyen> GetUngTuyenAsync(string maTK, int maBaiDang);
        Task XoaUngTuyenAsync(UngTuyen ungTuyen);
        Task<List<UngTuyen>> TimKiemUngTuyenAsync(string tieude, DateTime? thoiGian, string maGiaSu);
        Task<List<UngTuyen>> GetAllUngTuyenAsync();
        Task<List<UngTuyen>> GetAllUngTuyenAsyncByTrangThai(string trangthai);


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
       

        public async Task<List<UngTuyen>> GetAllUngTuyenAsyncByTrangThai(string trangthai)
        {
            return await _context.UngTuyen
                 .Include(ut => ut.TaiKhoanGiaSu)
                .Include(ut => ut.BaiDang)
                    .ThenInclude(bd => bd.TaiKhoan) // để truy xuất tác giả bài đăng
                .Include(ut => ut.HoSo)
                .Where(ut => ut.TrangThai == trangthai)
                .ToListAsync();
        }
        public async Task<List<UngTuyen>> GetAllUngTuyenAsync()
        {
            return await _context.UngTuyen
                 .Include(ut => ut.TaiKhoanGiaSu)
                .Include(ut => ut.BaiDang)
                    .ThenInclude(bd => bd.TaiKhoan) // để truy xuất tác giả bài đăng
                .Include(ut => ut.HoSo)
                .ToListAsync();
        }
        public async Task<List<UngTuyen>> LayUngVienTheoBaiDang(int maBaiDang) =>
        await _context.UngTuyen
            .Include(u => u.TaiKhoanGiaSu)
            .Include(u => u.HoSo)
            .Where(u => u.FK_iMaBaiDang == maBaiDang)
            .ToListAsync();

        public async Task<List<UngTuyen>> LayUngTuyenCuaGiaSu(string maTK,string trangthai)
        {
            return await _context.UngTuyen
                .Include(u => u.BaiDang)
                    .ThenInclude(bd => bd.TaiKhoan) // lấy thông tin phụ huynh
                .Include(u => u.HoSo)
                .Where(u => u.FK_iMaTK_GiaSu == maTK && u.TrangThai==trangthai)
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
        public async Task<UngTuyen> GetUngTuyenByIdAsync(int id)
        {
            return await _context.UngTuyen
                .Include(u => u.TaiKhoanGiaSu) // để lấy email
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<List<UngTuyen>> GetUngTuyenByBaiDangId(int baiDangId)
        {
            return await _context.UngTuyen
                .Include(u => u.TaiKhoanGiaSu) // để lấy email của từng ứng viên
                .Where(u => u.FK_iMaBaiDang == baiDangId)
                .ToListAsync();
        }
        public async Task<bool> CheckGiaSuHasAppliedAsync(string maTK, int maBaiDang)
        {
            // Kiểm tra trong bảng UngTuyen xem gia sư đã ứng tuyển vào bài đăng chưa
            return await _context.UngTuyen
                                 .AnyAsync(u => u.FK_iMaTK_GiaSu == maTK && u.FK_iMaBaiDang == maBaiDang);
        }

        public async Task<UngTuyen> GetUngTuyenAsync(string maTK, int maBaiDang)
        {
            return await _context.UngTuyen
                .FirstOrDefaultAsync(x => x.FK_iMaTK_GiaSu == maTK && x.FK_iMaBaiDang == maBaiDang);
        }

        public async Task XoaUngTuyenAsync(UngTuyen ungTuyen)
        {
            _context.UngTuyen.Remove(ungTuyen);
            await _context.SaveChangesAsync();
        }
            public async Task<List<UngTuyen>> TimKiemUngTuyenAsync(string tieude, DateTime? thoiGian, string maGiaSu)
            {
                var query = _context.UngTuyen
                    .Include(u => u.BaiDang)
                    .Include(u => u.HoSo)
                    .Include(u => u.TaiKhoanGiaSu)
                    .Include(u => u.BaiDang.TaiKhoan)
                    .Where(u => u.FK_iMaTK_GiaSu == maGiaSu)
                    .AsQueryable();
           

            if (thoiGian.HasValue)
            {
                query = query.Where(u => u.NgayUngTuyen.Date == thoiGian.Value.Date);
            }

            var list = await query.ToListAsync();

            if (!string.IsNullOrWhiteSpace(tieude))
            {
                var keyword = RemoveDiacritics(tieude).ToLower();
                list = list.Where(u => RemoveDiacritics(u.BaiDang.sTieuDe).ToLower().Contains(keyword)).ToList();
            }
            return  list;
            }

        public static string RemoveDiacritics(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var normalized = input.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
    }
