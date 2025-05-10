using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;
namespace WebApplication1.Repositories
{
    public interface IDangTinRepository
    {
        // Define methods for DangTin repository
        // For example:
        // Task<IEnumerable<DangTin>> GetAllDangTinAsync();
        // Task<DangTin> GetDangTinByIdAsync(int id);
        // Task AddDangTinAsync(DangTin dangTin);
        // Task UpdateDangTinAsync(DangTin dangTin);
        // Task DeleteDangTinAsync(int id);
        Task<bool> SaveChangesAsync();
        Task<bool> CreatDangTin(BaiDang dangTin);
        Task<BaiDang> GetDangTinById(int id);
        Task<bool> UpdateDangTin(BaiDang dangTin);
        Task<bool> DeleteDangTin(int id);
        Task<bool> DangTinExistsAsync(int id);
        Task<bool> DangTinExistsByTitleAsync(string title);
        Task<List<BaiDang>> GetAllBaiDangByTaiKhoanId(string taiKhoanId,string trangthai);
        Task<BaiDang> GetBaiDangById(int id);
        Task<List<BaiDang>> GetAllBaiDangByTrangthai(string trangthai);
        Task<bool> UpdateTrangThaiBaiDang(int id, string trangThai);

        Task<List<BaiDang>> GetAllBaiDangByVaiTroPhuHuynh();
        IQueryable<BaiDang> GetAll();
        Task<List<BaiDang>> GetAllBaiDangByTrangthaiGD(string trangthai);
        void CapNhatThoiGianDang(int baiDangId, DateTime den);
        Task<List<BaiDang>> GetBaiDangsAsync(string monhoc);
    }
    public class DangTinRepository : IDangTinRepository
    {
      public  DangTinRepository() { }
        private readonly ApplicationDbContext _context;
    
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        public DangTinRepository(ApplicationDbContext context, IAccountRepository accountRepository, UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager)
        {
            _context = context;
           
            _accountRepository = accountRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public DangTinRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreatDangTin(BaiDang dangTin)
        {
            await _context.BaiDangs.AddAsync(dangTin);
            return await SaveChangesAsync();
        }
        public async Task<BaiDang> GetDangTinById(int id)
        {
            return await _context.BaiDangs.FindAsync(id);
        }
        public async Task<bool> UpdateDangTin(BaiDang dangTin)
        {
            _context.BaiDangs.Update(dangTin);
            return await SaveChangesAsync();
        }
        public async Task<bool> DeleteDangTin(int id)
        {
            var dangTin = await GetDangTinById(id);
            if (dangTin == null) return false;
            _context.BaiDangs.Remove(dangTin);
            return await SaveChangesAsync();
        }
        public async Task<bool> DangTinExistsAsync(int id)
        {
            return await _context.BaiDangs.AnyAsync(e => e.PK_iMaBaiDang == id);
        }
        public async Task<bool> DangTinExistsByTitleAsync(string title)
        {
            return await _context.BaiDangs.AnyAsync(e => e.sTieuDe == title);
        }
      
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<BaiDang>> GetAllBaiDangByTaiKhoanId(string taiKhoanId,string trangthai)
        {
            var query = _context.BaiDangs
                .Include(b => b.TaiKhoan)
            .Where(b => b.FK_iMaTK == taiKhoanId); // Lọc theo TaiKhoanId

            if (!string.IsNullOrEmpty(trangthai)) // Nếu trạng thái được truyền vào, lọc thêm theo trạng thái
            {
                query = query.Where(b => b.sTrangThai == trangthai);
            }
            return await query.ToListAsync();
        }
        public async Task<List<BaiDang>> GetAllBaiDangByTrangthai(string trangthai)
        {

            return await  _context.BaiDangs
                .Include(b => b.TaiKhoan)
            .Where(b => b.sTrangThai == trangthai).ToListAsync(); // Lọc theo Trang thai
        
        }
        public async Task<BaiDang> GetBaiDangById(int id)
          {
             
              return await _context.BaiDangs
                 .Include(b => b.TaiKhoan)
                .FirstOrDefaultAsync(bd => bd.PK_iMaBaiDang==id) ;
              
          }
        public async Task<bool> UpdateTrangThaiBaiDang(int id, string trangThai)
        {
            var baiDang = await _context.BaiDangs.FindAsync(id);
            if (baiDang == null) return false;

            baiDang.sTrangThai = trangThai;
            return await SaveChangesAsync();
        }
        public async Task<List<BaiDang>> GetAllBaiDangByVaiTroPhuHuynh()
        {

            return await _context.BaiDangs
             .Include(b => b.TaiKhoan)
             .Where(b => b.sTrangThai == "Đã duyệt"
                      && b.TaiKhoan.VaiTro == "phuhuynh"
                      && !b.IsHidden)
             .ToListAsync();
        }
        public IQueryable<BaiDang> GetAll()
        {
            return _context.BaiDangs.AsQueryable();
        }
        public void CapNhatThoiGianDang(int baiDangId, DateTime den)
        {
            var baiDang = _context.BaiDangs.FirstOrDefault(b => b.PK_iMaBaiDang == baiDangId);
            if (baiDang != null)
            {
                baiDang.dUuTienDen = den;
                _context.SaveChanges();
            }
        }
        public async Task<List<BaiDang>> GetAllBaiDangByTrangthaiGD(string trangthai)
        {

            return await _context.BaiDangs
                .Include(b => b.TaiKhoan)
            .Where(b => b.sTrangThaiGD == trangthai).ToListAsync(); // Lọc theo Trang thai

        }
        public async Task<List<BaiDang>> GetBaiDangsAsync(string monhoc)
        {
            var query = _context.BaiDangs
                .Include(b => b.TaiKhoan)
                .Where(b => b.sTrangThai == "Đã duyệt"
                            && b.TaiKhoan.VaiTro == "phuhuynh"
                            && !b.IsHidden);

            if (!string.IsNullOrEmpty(monhoc))
            {
                query = query.Where(b => b.sMonday.Contains(monhoc));
            }

            return await query.ToListAsync();
        }



    }
}
