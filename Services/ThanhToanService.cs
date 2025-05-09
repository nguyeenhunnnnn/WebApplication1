using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public interface IThanhToanService
    {
        void ThanhToanGoi(string userId, int goiId, int baiDangId);
        List<ThanhToan> LayLichSu(string userId);
        Task<List<LichSuTTViewModel>> GetLichSuThanhToanAsync(string userId);
        Task<List<LichSuTTViewModel>> GetByTrangThaiAsync(bool isDuyet);
        Task DuyetThanhToanAsync(int thanhToanId);


    }
    public class ThanhToanService : IThanhToanService
    {
        private readonly IThanhToanRepository _thanhToanRepo;
        private readonly IDangTinRepository _baiDangRepo;

        public ThanhToanService(IThanhToanRepository thanhToanRepo, IDangTinRepository baiDangRepo)
        {
            _thanhToanRepo = thanhToanRepo;
            _baiDangRepo = baiDangRepo;
        }

        public void ThanhToanGoi(string userId, int goiId, int baiDangId)
        {
            var thanhToan = new ThanhToan
            {
                TaiKhoanId = userId,
                GoiDichVuId = goiId,
                NgayThanhToan = DateTime.Now,
                BaiDangId = baiDangId,
            };

            _thanhToanRepo.Them(thanhToan);
            //_baiDangRepo.CapNhatThoiGianDang(baiDangId, DateTime.Now);
        }

        public List<ThanhToan> LayLichSu(string userId)
        {
            return _thanhToanRepo.GetByUser(userId);
        }
        public async Task<List<LichSuTTViewModel>> GetLichSuThanhToanAsync(string userId)
        {
            var lichSu = await _thanhToanRepo.GetLichSuByUserIdAsync(userId);

            return lichSu.Select(t => new LichSuTTViewModel
            {
                Id = t.Id,
                TenGoi = t.GoiDichVu.TenGoi,
                NgayThanhToan = t.NgayThanhToan,
                SoTien = t.GoiDichVu.Gia,
                // Loai = t.BaiDangId != null ? "Bài đăng" : "Hồ sơ",
                //BaiDangId = t.BaiDangId,
                //HoSoId = t.HoSoId
                TenNguoiDung = t.TaiKhoan.UserName,
                TieuDeBaiDang = t.BaiDang.sTieuDe,
                NgayDang = t.BaiDang.dNgayTao,
                DayUuTienDen = t.BaiDang.dUuTienDen,
                TrangThaiGD = t.BaiDang.sTrangThaiGD
            }).ToList();
        }
        /*public async Task<List<LichSuTTViewModel>> GetAllLichSuThanhToanAsync()
        {
            var lichSu = await _thanhToanRepo.GetAllLichSuByUserIdAsync();

            return lichSu.Select(t => new LichSuTTViewModel
            {
                Id = t.Id,
                TenGoi = t.GoiDichVu.TenGoi,
                NgayThanhToan = t.NgayThanhToan,
                SoTien = t.GoiDichVu.Gia,
                // Loai = t.BaiDangId != null ? "Bài đăng" : "Hồ sơ",
                //BaiDangId = t.BaiDangId,
                //HoSoId = t.HoSoId
                TenNguoiDung = t.TaiKhoan.UserName,
                TieuDeBaiDang = t.BaiDang.sTieuDe,
                NgayDang = t.BaiDang.dNgayTao,
                DayUuTienDen = t.BaiDang.dUuTienDen

            }).ToList();
        }*/
        public async Task<List<LichSuTTViewModel>> GetByTrangThaiAsync(bool isDuyet)
        {
            var list = await _thanhToanRepo.GetByTrangThaiAsync(isDuyet);
            return list.Select(t => new LichSuTTViewModel
            {
                Id = t.Id,
                TenNguoiDung = t.TaiKhoan.UserName,
                TieuDeBaiDang = t.BaiDang.sTieuDe,
                TenGoi = t.GoiDichVu.TenGoi,
                SoTien = t.GoiDichVu.Gia,
                NgayThanhToan = t.NgayThanhToan,
                DayUuTienDen = t.BaiDang.dUuTienDen,
                TrangThaiGD = t.BaiDang.sTrangThaiGD
            }).ToList();
        }

        public async Task DuyetThanhToanAsync(int thanhToanId)
        {
            await _thanhToanRepo.DuyetThanhToanAsync(thanhToanId);
        }

    }
}
