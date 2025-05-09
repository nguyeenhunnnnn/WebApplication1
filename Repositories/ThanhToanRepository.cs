using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Entities;

namespace WebApplication1.Repositories
{
    public interface IThanhToanRepository
    {
        void Them(ThanhToan thanhToan);
        List<ThanhToan> GetByUser(string userId);

        Task<List<ThanhToan>> GetLichSuByUserIdAsync(string userId);
        Task<List<ThanhToan>> GetAllLichSuAsync(bool? isDuyet = null);

        Task<List<ThanhToan>> GetByTrangThaiAsync(bool isDuyet);
        Task DuyetThanhToanAsync(int thanhToanId);
    }
    public class ThanhToanRepository : IThanhToanRepository
    {
        private readonly ApplicationDbContext _context;

        public ThanhToanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Them(ThanhToan thanhToan)
        {
            _context.ThanhToans.Add(thanhToan);
            _context.SaveChanges();
        }

        public List<ThanhToan> GetByUser(string userId)
        {
            return _context.ThanhToans
                .Include(t => t.GoiDichVu)
                .Include(t => t.BaiDang)
                .Where(t => t.TaiKhoanId == userId)
                .OrderByDescending(t => t.NgayThanhToan)
                .ToList();
        }
        public async Task<List<ThanhToan>> GetLichSuByUserIdAsync(string userId)
        {
            return await _context.ThanhToans
                .Include(t => t.GoiDichVu)
                .Include(t => t.BaiDang)
                .Include(t => t.TaiKhoan)
                .Where(t => t.TaiKhoanId == userId)
                .OrderByDescending(t => t.NgayThanhToan)
                .ToListAsync();
        }

        /*public async Task<List<ThanhToan>> GetAllLichSuByUserIdAsync()
        {
            return await _context.ThanhToans
                .Include(t => t.GoiDichVu)
                .Include(t => t.BaiDang)
                .Include(t => t.TaiKhoan)
                .OrderByDescending(t => t.NgayThanhToan)
                .ToListAsync();
        }*/
        public async Task<List<ThanhToan>> GetAllLichSuAsync(bool? isDuyet = null)
        {
            var query = _context.ThanhToans
                .Include(t => t.GoiDichVu)
                .Include(t => t.BaiDang)
                .Include(t => t.TaiKhoan)
                .AsQueryable();

            if (isDuyet.HasValue)
            {
                query = query.Where(t => t.IsDuyet == isDuyet.Value);
            }

            return await query.OrderByDescending(t => t.NgayThanhToan).ToListAsync();
        }
        public async Task<List<ThanhToan>> GetByTrangThaiAsync(bool isDuyet)
        {
            return await _context.ThanhToans
                .Include(t => t.TaiKhoan)
                .Include(t => t.BaiDang)
                .Include(t => t.GoiDichVu)
                .Where(t => t.IsDuyet == isDuyet)
                .ToListAsync();
        }

        public async Task DuyetThanhToanAsync(int thanhToanId)
        {
            var thanhToan = await _context.ThanhToans
                .Include(t => t.BaiDang)
                .Include(t => t.GoiDichVu)
                .Include(t => t.TaiKhoan)
                .FirstOrDefaultAsync(t => t.Id == thanhToanId);

            if (!thanhToan.IsDuyet)
            {
                thanhToan.IsDuyet = true;
                thanhToan.BaiDang.sTrangThaiGD = "Đã duyệt";
                thanhToan.NgayThanhToan = DateTime.Now;
                thanhToan.BaiDang.dUuTienDen = DateTime.Now.AddDays(thanhToan.GoiDichVu.SoNgayHieuLuc);
                thanhToan.TaiKhoan.GoiCuoc = true;

                await _context.SaveChangesAsync();
            }
        }
    }
}

