using System;
using WebApplication1.Data;
using WebApplication1.Models.ViewModels;
using WebApplication1.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Globalization;
using System.Text;
namespace WebApplication1.Services
{
    public interface IUngTuyenService
    {
        Task UngTuyenAsync(string maTK, int maBaiDang);
        Task<List<UngTuyenViewModel>> LayDanhSachUngVien(int maBaiDang);
        Task DuyetUngVien(int id, string trangThai);
        Task<List<UngTuyenViewModel>> LayDanhSachUngTuyenCuaGiaSu(string maTK, string trangthai);
        Task<List<UngTuyenViewModel>> LayDanhSachUngVienCuaPhuHuynh(string phuHuynhId, string trangthai);
        Task HuyUngTuyenAsync(string maTK, int maBaiDang);
        Task<List<UngTuyenViewModel>> TimKiemHoSoAsync(string tieude, DateTime? thoiGian, string maGiaSu);
        Task<List<UngTuyenViewModel>> GetAllUngTuyenAsync(string trangthai);
        Task<List<UngTuyenViewModel>> TimKiemUngTuyenAsync(string tuKhoa, DateTime? thoiGian);
    }
    public class UngTuyenService : IUngTuyenService
    {
        private readonly IUngTuyenRepository _repo;
        private readonly ApplicationDbContext _context;
        private readonly IHoSoRepository _hosoRepo;

        public UngTuyenService(IUngTuyenRepository repo, IHoSoRepository hosoRepo, ApplicationDbContext context)
        {
            _repo = repo;
            _hosoRepo = hosoRepo;
            _context = context;
        }
        public async Task<List<UngTuyenViewModel>> TimKiemUngTuyenAsync(string tuKhoa, DateTime? thoiGian)
        {
            var query = _context.UngTuyen
                .Include(ut => ut.TaiKhoanGiaSu)
                .Include(ut => ut.BaiDang).ThenInclude(bd => bd.TaiKhoan)
                .Include(ut => ut.HoSo)
                .AsQueryable();

            // Lọc theo ngày (vẫn có thể xử lý ở SQL)
            if (thoiGian.HasValue)
            {
                query = query.Where(ut => ut.NgayUngTuyen.Date == thoiGian.Value.Date);
            }

            // Thực thi SQL trước, rồi lọc theo từ khóa ở client
            var list = await query.ToListAsync();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = RemoveDiacritics(tuKhoa).ToLower();
                list = list.Where(ut =>
                    RemoveDiacritics(ut.TaiKhoanGiaSu.UserName ?? "").ToLower().Contains(keyword) ||
                    RemoveDiacritics(ut.BaiDang?.sTieuDe ?? "").ToLower().Contains(keyword) ||
                    RemoveDiacritics(ut.HoSo?.sTieuDe ?? "").ToLower().Contains(keyword)
                ).ToList();
            }

            return list.Select(ut => new UngTuyenViewModel
            {
                Id = ut.Id,
                TenGiaSu = ut.TaiKhoanGiaSu.UserName,
                GiaSuID = ut.TaiKhoanGiaSu.Id,
                EmailGiaSu = ut.TaiKhoanGiaSu.Email,
                AvatarUrl = ut.TaiKhoanGiaSu.FileAvata,
                IdHoSo = ut.HoSo.iMaHS,
                TieuDeHoSo = ut.HoSo.sTieuDe,
                BangCap = ut.HoSo.sBangCap,
                phuhuynhName = ut.BaiDang.TaiKhoan.UserName,
                PhuHuynhId = ut.BaiDang.TaiKhoan.Id,
                PhuHuynhAvatarUrl=ut.BaiDang.TaiKhoan.FileAvata,
                TrangThai = ut.TrangThai,
                NgayUngTuyen = ut.NgayUngTuyen,
                MaBaiDang = ut.BaiDang.PK_iMaBaiDang,
                TieuDeBaiDang = ut.BaiDang.sTieuDe,
                fileCV = ut.HoSo.sDuongDanTep,
                ishidden = ut.BaiDang.IsHidden
            }).ToList();
        }


        public async Task<List<UngTuyenViewModel>> GetAllUngTuyenAsync(string trangthai)
        {
            var list = await _repo.GetAllUngTuyenAsyncByTrangThai(trangthai);
            return list.Select(u => new UngTuyenViewModel
            {
                Id = u.Id,
                TenGiaSu = u.TaiKhoanGiaSu.UserName,
                GiaSuID = u.TaiKhoanGiaSu.Id,
                EmailGiaSu = u.TaiKhoanGiaSu.Email,
                AvatarUrl = u.TaiKhoanGiaSu.FileAvata,
                TieuDeHoSo = u.HoSo.sTieuDe,
                BangCap = u.HoSo.sBangCap,
                IdHoSo = u.HoSo.iMaHS,
                phuhuynhName = u.BaiDang.TaiKhoan.UserName,
                PhuHuynhId = u.BaiDang.TaiKhoan.Id,
                PhuHuynhAvatarUrl = u.BaiDang.TaiKhoan.FileAvata,
                TrangThai = u.TrangThai,
                NgayUngTuyen = u.NgayUngTuyen,
                fileCV = u.HoSo.sDuongDanTep,
                ishidden = u.BaiDang.IsHidden,
                TieuDeBaiDang=u.BaiDang.sTieuDe,
                MaBaiDang = u.FK_iMaBaiDang
            }).ToList();
        }
        public async Task UngTuyenAsync(string maTK, int maBaiDang)
        {
           
            var hoSo = await _hosoRepo.LayHoSoMoiNhatCuaGiaSu(maTK);
            var ungTuyen = new UngTuyen
            {
                FK_iMaTK_GiaSu = maTK,
                FK_iMaBaiDang = maBaiDang,
                FK_iMaHS = hoSo.iMaHS,
                TrangThai = "Chờ duyệt", // Hoặc trạng thái mặc định khác
            };
            await _repo.ThemUngTuyenAsync(ungTuyen);
        }
        public async Task HuyUngTuyenAsync(string maTK, int maBaiDang)
        {
            var ungTuyen = await _repo.GetUngTuyenAsync(maTK, maBaiDang);

            if (ungTuyen == null)
                throw new InvalidOperationException("Không tìm thấy thông tin ứng tuyển để hủy.");

            await _repo.XoaUngTuyenAsync(ungTuyen);
        }
        public async Task<List<UngTuyenViewModel>> LayDanhSachUngVien(int maBaiDang)
        {
            var list = await _repo.LayUngVienTheoBaiDang(maBaiDang);
            return list.Select(u => new UngTuyenViewModel
            {
                Id = u.Id,
                TenGiaSu = u.TaiKhoanGiaSu.UserName,
                GiaSuID = u.TaiKhoanGiaSu.Id,
                EmailGiaSu = u.TaiKhoanGiaSu.Email,
                AvatarUrl = u.TaiKhoanGiaSu.FileAvata,
                TieuDeHoSo = u.HoSo.sTieuDe,
                BangCap = u.HoSo.sBangCap,
                IdHoSo = u.HoSo.iMaHS,
                phuhuynhName=u.BaiDang.TaiKhoan.UserName,
                PhuHuynhId = u.BaiDang.TaiKhoan.Id,
                PhuHuynhAvatarUrl = u.BaiDang.TaiKhoan.FileAvata,
                TrangThai = u.TrangThai,
                NgayUngTuyen = u.NgayUngTuyen,
                fileCV = u.HoSo.sDuongDanTep,
                ishidden = u.BaiDang.IsHidden,
                MaBaiDang = u.FK_iMaBaiDang
            }).ToList();
        }

        public async Task DuyetUngVien(int id, string trangThai)
            => await _repo.CapNhatTrangThaiAsync(id, trangThai);

        public async Task<List<UngTuyenViewModel>> LayDanhSachUngTuyenCuaGiaSu(string maTK, string trangthai)
        {
            var list = await _repo.LayUngTuyenCuaGiaSu(maTK,trangthai);
            return list.Select(u => new UngTuyenViewModel
            {
                Id = u.Id,
                GiaSuID = u.TaiKhoanGiaSu.Id,
                TieuDeHoSo = u.HoSo?.sTieuDe,
                MaBaiDang = u.FK_iMaBaiDang,
                TieuDeBaiDang = u.BaiDang?.sTieuDe,
                phuhuynhName = u.BaiDang?.TaiKhoan?.UserName,
                PhuHuynhId = u.BaiDang.TaiKhoan.Id,
                PhuHuynhAvatarUrl = u.BaiDang.TaiKhoan.FileAvata,
                TrangThai = u.TrangThai,
                NgayUngTuyen = u.NgayUngTuyen
            }).ToList();
        }
        public async Task<List<UngTuyenViewModel>> LayDanhSachUngVienCuaPhuHuynh(string phuHuynhId, string trangthai)
        {
            var list = await _repo.LayDanhSachUngVienCuaPhuHuynh(phuHuynhId,trangthai);
            return list.Select(ut => new UngTuyenViewModel
            {
                Id = ut.Id,
                GiaSuID=ut.TaiKhoanGiaSu.Id,
                TenGiaSu = ut.TaiKhoanGiaSu.UserName,
                EmailGiaSu = ut.TaiKhoanGiaSu.Email,
                AvatarUrl = ut.TaiKhoanGiaSu.FileAvata,
                fileCV = ut.HoSo.sDuongDanTep,
                IdHoSo = ut.HoSo.iMaHS,
                MaBaiDang = ut.FK_iMaBaiDang,
                TieuDeHoSo = ut.HoSo.sTieuDe,
                TieuDeBaiDang = ut.BaiDang.sTieuDe,
                NgayUngTuyen = ut.NgayUngTuyen,
                ishidden = ut.BaiDang.IsHidden,
                TrangThai = ut.TrangThai
            }).ToList();
        }

        public async Task<List<UngTuyenViewModel>> TimKiemHoSoAsync(string tieude, DateTime? thoiGian, string maGiaSu)
        {
            var danhSach = await _repo.TimKiemUngTuyenAsync(tieude, thoiGian, maGiaSu);
          
            return danhSach.Select(u => new UngTuyenViewModel
            {
                Id = u.Id,
                TenGiaSu = u.TaiKhoanGiaSu?.UserName,
                GiaSuID = u.FK_iMaTK_GiaSu,
                EmailGiaSu = u.TaiKhoanGiaSu?.Email,
                AvatarUrl = u.TaiKhoanGiaSu?.FileAvata,
                IdHoSo = u.FK_iMaHS,
                TieuDeHoSo = u.HoSo?.sTieuDe,
                BangCap = u.HoSo?.sBangCap,
                phuhuynhName = u.BaiDang?.TaiKhoan?.UserName,
                PhuHuynhId = u.BaiDang?.FK_iMaTK,
                TrangThai = u.TrangThai,
                NgayUngTuyen = u.NgayUngTuyen,
                MaBaiDang = u.BaiDang?.PK_iMaBaiDang ?? 0,
                TieuDeBaiDang = u.BaiDang?.sTieuDe,
                fileCV = u.HoSo?.sDuongDanTep,
                ishidden = u.BaiDang?.IsHidden ?? false
            }).ToList();
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
