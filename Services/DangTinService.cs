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
using NuGet.Protocol.Core.Types;
using System.Globalization;
using System.Text;

namespace WebApplication1.Services
{
    public interface IDangTinService
    {
        // Define methods for DangTin service
        // For example:
        // Task<IEnumerable<DangTin>> GetAllDangTinAsync();
        // Task<DangTin> GetDangTinByIdAsync(int id);
        // Task AddDangTinAsync(DangTin dangTin);
        // Task UpdateDangTinAsync(DangTin dangTin);
        // Task DeleteDangTinAsync(int id);
        Task<bool> SaveChangesAsync();
        Task<bool> CreatDangTin(DangTinViewModel model, string userId);
        Task<BaiDang> GetDangTinById(int id);
        Task<bool> UpdateDangTin(BaiDangViewModel model);
        Task<bool> DeleteDangTin(int id);
        Task<bool> DangTinExistsAsync(int id);
        Task<bool> DangTinExistsByTitleAsync(string title);
        Task<List<BaiDangViewModel>> GetAllBaiDangByTaiKhoanId(string taiKhoanId,string trangthai);
        Task<BaiDangViewModel> GetBaiDangById(int id);
        Task<List<BaiDangViewModel>> GetAllBaiDangByTrangthai(string trangthai);
        Task<bool> PheDuyetBaiDangAsync(int id);
        Task<bool> CreateDangTinForGiaSuAsync(DangTinGiaSuViewModel model, string userId);
        Task<BaiDangViewModel> GetBaiDangByIdGS(int id);
        Task HideBaiDang(int baiDangId, bool isHidden);

        Task<List<BaiDangViewModel>> GetAllBaiDangByVaiTroPhuHuynh();
        Task<List<BaiDangViewModel>> TimKiemBaiDangAsync(string Keyword, string MonHoc, string diadiem, string MucLuong, string KinhNghiem);
    }
    public class DangTinService : IDangTinService
    {
      public  DangTinService() { }
        private readonly ApplicationDbContext _context;
        private readonly IDangTinRepository _dangTinRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly IHoSoRepository _hoSoRepository;
        private readonly IDanhGiaService _danhGiaService;
        public DangTinService(ApplicationDbContext context, IDangTinRepository dangTinRepository, IAccountRepository accountRepository, UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager, IHoSoRepository hoSoRepository, IDanhGiaService danhGiaService)
        {
            _context = context;
            _dangTinRepository = dangTinRepository;
            _accountRepository = accountRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _hoSoRepository = hoSoRepository;
            _danhGiaService = danhGiaService;

        }
        public async Task<bool> CreatDangTin(DangTinViewModel model, string userId)
        {
            var dangTin = new BaiDang(); 
            dangTin.sMonday = model.sMonday;
            dangTin.sYCau = model.sYCau;
            dangTin.sGioiTinh = model.sGioiTinh;
            dangTin.sTuoi = model.sTuoi;
            dangTin.sKinhNghiem = model.sKinhNghiem;
            dangTin.sBangCap = model.sBangCap;
            dangTin.sTieuDe = model.sTieuDe;
            dangTin.sDiaDiem = model.sDiaDiem;
            dangTin.fMucLuong = model.fMucLuong;
            dangTin.dThoiGianHetHan = model.dThoiGianHetHan;
            dangTin.sMoTa = model.sMoTa;
            dangTin.dNgayTao = DateTime.Now;
            dangTin.sTrangThai = "Đang chờ duyệt";
            dangTin.FK_iMaTK = userId;
            try
            {
                await _dangTinRepository.CreatDangTin(dangTin);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Log the exception or handle it as needed
                return false;
            }
            
        }
        public async Task<BaiDang> GetDangTinById(int id)
        {
            return await _dangTinRepository.GetDangTinById(id);
        }
        public async Task<bool> UpdateDangTin(BaiDangViewModel model )
        {
            var dangTin = await _dangTinRepository.GetBaiDangById(model.PK_iMaBaiDang);
            if (dangTin == null)
            {
                return false;
            }
            dangTin.sTieuDe = model.sTieuDe;
            dangTin.sMoTa = model.sMoTa;
            dangTin.sDiaDiem = model.sDiaDiem;
            dangTin.fMucLuong = model.fMucLuong;
            dangTin.dThoiGianHetHan = model.dThoiGianHetHan;
            dangTin.dNgayTao = DateTime.Now;
            dangTin.sMonday = model.sMonday;
            dangTin.sYCau = model.sYCau;
            dangTin.sGioiTinh = model.sGioiTinh;
            dangTin.sTuoi = model.sTuoi;
            dangTin.sKinhNghiem = model.sKinhNghiem;
            dangTin.sBangCap = model.sBangCap;
            return await _dangTinRepository.UpdateDangTin(dangTin);
        }
        public async Task<bool> DeleteDangTin(int id)
        {
            return await _dangTinRepository.DeleteDangTin(id);
        }
        public async Task<bool> DangTinExistsAsync(int id)
        {
            return await _context.BaiDangs.AnyAsync(d => d.PK_iMaBaiDang == id);
        }
        public async Task<bool> DangTinExistsByTitleAsync(string title)
        {
            return await _dangTinRepository.DangTinExistsByTitleAsync(title);
        }
        
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DangTinExistsById(int id)
        {
            return await _context.BaiDangs.AnyAsync(d => d.PK_iMaBaiDang == id);
        }
        public async Task<bool> DangTinExistsByTitle(string title)
        {
            return await _context.BaiDangs.AnyAsync(d => d.sTieuDe == title);
        }
        public async Task<bool> DangTinExistsByLocation(string location)
        {
            return await _context.BaiDangs.AnyAsync(d => d.sDiaDiem == location);
        }
        public async Task<bool> DangTinExistsByIdAsync(int id)
        {
            return await _context.BaiDangs.AnyAsync(d => d.PK_iMaBaiDang == id);
        }
        public async Task<List<BaiDangViewModel>> GetAllBaiDangByTaiKhoanId(string taiKhoanId,string trangthai)
        {
            var baiDangs = await _dangTinRepository.GetAllBaiDangByTaiKhoanId(taiKhoanId,trangthai);

           return baiDangs.Select(b=>new BaiDangViewModel
                {
                    PK_iMaBaiDang = b.PK_iMaBaiDang,
                    Nguoitao = b.TaiKhoan.UserName,
                     sMonday =b.sMonday,
                    sBangCap=b.sBangCap,
                    sGioiTinh=b.sGioiTinh,
                    sKinhNghiem=b.sKinhNghiem,
                    sTuoi=b.sTuoi,
                    sYCau=b.sYCau,
                    sTieuDe = b.sTieuDe,
                    sMoTa = b.sMoTa,
                    sDiaDiem = b.sDiaDiem,
                    fMucLuong = b.fMucLuong ?? 0,
                    sTrangThai = b.sTrangThai,
                    dNgayTao = b.dNgayTao,
                    dThoiGianHetHan = b.dThoiGianHetHan ?? DateTime.MinValue,
                    FileCVPath = b.FileCVPath // Gán file CV từ hồ sơ nếu có
           })
                .ToList();
          
        }
        public async Task<List<BaiDangViewModel>> GetAllBaiDangByTrangthai( string trangthai)
        {
            var baiDangs = await _dangTinRepository.GetAllBaiDangByTrangthai( trangthai);

            return baiDangs.Select(b => new BaiDangViewModel
            {
                PK_iMaBaiDang = b.PK_iMaBaiDang,
                Nguoitao = b.TaiKhoan.UserName,
                sMonday = b.sMonday,
                sBangCap = b.sBangCap,
                sGioiTinh = b.sGioiTinh,
                sKinhNghiem = b.sKinhNghiem,
                sTuoi = b.sTuoi,
                sYCau = b.sYCau,
                sTieuDe = b.sTieuDe,
                sMoTa = b.sMoTa,
                sDiaDiem = b.sDiaDiem,
                fMucLuong = b.fMucLuong ?? 0,
                sTrangThai = b.sTrangThai,
                dNgayTao = b.dNgayTao,
                FileCVPath = b.FileCVPath, // Gán file CV từ hồ sơ nếu có
                dThoiGianHetHan = b.dThoiGianHetHan ?? DateTime.MinValue,

            })
                 .ToList();

        }

        public async Task<BaiDangViewModel> GetBaiDangById(int id)
        {

           var bd = await _dangTinRepository.GetBaiDangById(id);

            if (bd == null)
            {
                return null; // Hoặc xử lý lỗi theo cách bạn muốn
            }
            return new BaiDangViewModel
            {
                PK_iMaBaiDang = bd.PK_iMaBaiDang,
                Nguoitao = bd.TaiKhoan.UserName,
                sMonday = bd.sMonday,
                sBangCap = bd.sBangCap,
                sGioiTinh = bd.sGioiTinh,
                sKinhNghiem = bd.sKinhNghiem,
                sTuoi = bd.sTuoi,
                sYCau = bd.sYCau,
                sTieuDe = bd.sTieuDe,
                sMoTa = bd.sMoTa,
                sDiaDiem = bd.sDiaDiem,
                fMucLuong = bd.fMucLuong ?? 0,
                sTrangThai = bd.sTrangThai,
                dNgayTao = bd.dNgayTao,
                dThoiGianHetHan = bd.dThoiGianHetHan ?? DateTime.MinValue,
                FileCVPath = bd.FileCVPath // Gán file CV từ hồ sơ nếu có
            };


        }
        public async Task<BaiDangViewModel> GetBaiDangByIdGS(int id)
        {

            var bd = await _dangTinRepository.GetBaiDangById(id);
            


            if (bd == null)
            {
                return null; // Hoặc xử lý lỗi theo cách bạn muốn
            }
            if (string.IsNullOrEmpty(bd.FK_iMaTK))
            {
                return null; // hoặc xử lý hợp lý
            }

           
            var danhGias = await _danhGiaService.GetDanhGiaByGiaSuIdAsync(bd.TaiKhoan.Id);
            var diemTB = await _danhGiaService.GetAverageRatingAsync(bd.TaiKhoan.Id);
            return new BaiDangViewModel
            {
                PK_iMaBaiDang = bd.PK_iMaBaiDang,
                Nguoitao = bd.TaiKhoan.UserName,
                sMonday = bd.sMonday,
                sBangCap = bd.sBangCap,
                sGioiTinh = bd.sGioiTinh,
                sKinhNghiem = bd.sKinhNghiem,
                sTuoi = bd.sTuoi,
                sYCau = bd.sYCau,
                sTieuDe = bd.sTieuDe,
                sMoTa = bd.sMoTa,
                sDiaDiem = bd.sDiaDiem,
                fMucLuong = bd.fMucLuong ?? 0,
                sTrangThai = bd.sTrangThai,
                dNgayTao = bd.dNgayTao,
                dThoiGianHetHan = bd.dThoiGianHetHan ?? DateTime.MinValue,
                FileCVPath = bd.FileCVPath, // Gán file CV từ hồ sơ nếu có

                // thêm danh gia
                // ➕ Truyền dữ liệu đánh giá vào ViewModel
                DanhGias = danhGias,
                DiemTrungBinh = diemTB
            };


        }
        public async Task<bool> PheDuyetBaiDangAsync(int id)
        {
            var baiDang = await _dangTinRepository.GetDangTinById(id);
            if (baiDang == null) return false;

            return await _dangTinRepository.UpdateTrangThaiBaiDang(id, "Đã duyệt");
        }
        public async Task<bool> CreateDangTinForGiaSuAsync(DangTinGiaSuViewModel model, string userId)
        {

            
            var hoSoGiaSu = await _hoSoRepository.LayHoSoMoiNhatCuaGiaSu(userId);

            if (hoSoGiaSu == null || string.IsNullOrEmpty(hoSoGiaSu.sDuongDanTep)) // Kiểm tra nếu không có CV
            {
                // Nếu chưa có hồ sơ hoặc chưa có CV, không cho đăng bài
                return false;
            }

            var baiDang = new BaiDang
            {
                FK_iMaTK = userId,
                sTieuDe = model.sTieuDe ?? hoSoGiaSu.sTieuDe, // Sử dụng tiêu đề từ hồ sơ nếu không có từ form
                sMoTa = model.sMoTa,
                sDiaDiem = model.sDiaDiem,
                fMucLuong = model.fMucLuong,
                dNgayTao = DateTime.Now,
                dThoiGianHetHan = model.dThoiGianHetHan ?? DateTime.MinValue,
                sTrangThai = "Đang chờ duyệt",
                sMonday = model.sMonday,
                sYCau = model.sYCau,
                sGioiTinh = model.sGioiTinh,
                sTuoi = model.sTuoi,
                sKinhNghiem = model.sKinhNghiem ?? hoSoGiaSu.sKinhNghiem, // Lấy từ hồ sơ nếu không có từ form
                sBangCap = model.sBangCap ?? hoSoGiaSu.sBangCap, // Lấy từ hồ sơ nếu không có từ form
                FileCVPath = hoSoGiaSu.sDuongDanTep  // Gán file CV từ hồ sơ
            };

            return await _dangTinRepository.CreatDangTin(baiDang);
        }
        public async Task HideBaiDang(int baiDangId, bool isHidden)
        {
            var baiDang = await _dangTinRepository.GetBaiDangById(baiDangId);
            if (baiDang != null)
            {
                baiDang.IsHidden = isHidden;
                await _dangTinRepository.UpdateDangTin(baiDang);
            }
        }

        public async Task<List<BaiDangViewModel>> GetAllBaiDangByVaiTroPhuHuynh()
        {
            var listds = await _dangTinRepository.GetAllBaiDangByVaiTroPhuHuynh();
            return listds.Select(b => new BaiDangViewModel
            {
                PK_iMaBaiDang = b.PK_iMaBaiDang,
                Nguoitao = b.TaiKhoan.UserName,
                sMonday = b.sMonday,
                sBangCap = b.sBangCap,
                sGioiTinh = b.sGioiTinh,
                sKinhNghiem = b.sKinhNghiem,
                sTuoi = b.sTuoi,
                sYCau = b.sYCau,
                sTieuDe = b.sTieuDe,
                sMoTa = b.sMoTa,
                sDiaDiem = b.sDiaDiem,
                fMucLuong = b.fMucLuong ?? 0,
                sTrangThai = b.sTrangThai,
                dNgayTao = b.dNgayTao,
                FileCVPath = b.FileCVPath, // Gán file CV từ hồ sơ nếu có
                sfileAvata = b.TaiKhoan.FileAvata,
                dThoiGianHetHan = b.dThoiGianHetHan ?? DateTime.MinValue,
            })
                 .ToList();
           
        }
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var normalized = text.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    builder.Append(c);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC).ToLower();
        }

        public async Task<List<BaiDangViewModel>> TimKiemBaiDangAsync(string Keyword, string MonHoc, string diadiem, string MucLuong, string KinhNghiem)
        {
            var query = _dangTinRepository.GetAll()
                        .Include(x => x.TaiKhoan)
                        .Where(x => x.IsHidden == false
                        && x.TaiKhoan.VaiTro=="phuhuynh"
                        ); // ví dụ lọc bài đang hiển thị

            var data = await query.ToListAsync(); // Tải về bộ nhớ để xử lý bỏ dấu

            if (!string.IsNullOrWhiteSpace(Keyword))
            {
                var keywordNormalized = RemoveDiacritics(Keyword);
                data = data.Where(x =>
                    RemoveDiacritics(x.sTieuDe).Contains(keywordNormalized) ||
                    RemoveDiacritics(x.sMoTa).Contains(keywordNormalized) ||
                    RemoveDiacritics(x.sMonday).Contains(keywordNormalized)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(MonHoc))
            {
                var monHocNormalized = RemoveDiacritics(MonHoc);
                data = data.Where(x => RemoveDiacritics(x.sMonday).Contains(monHocNormalized)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(diadiem))
            {
                var diaDiemNormalized = RemoveDiacritics(diadiem);
                data = data.Where(x => RemoveDiacritics(x.sDiaDiem).Contains(diaDiemNormalized)).ToList();
            }

            if (!string.IsNullOrEmpty(MucLuong))
            {
                switch (MucLuong)
                {
                    case "Duoi1tr":
                        data = data.Where(x => x.fMucLuong < 1000000).ToList();
                        break;
                    case "1-3tr":
                        data = data.Where(x => x.fMucLuong >= 1000000 && x.fMucLuong <= 3000000).ToList();
                        break;
                    case "Tren3tr":
                        data = data.Where(x => x.fMucLuong > 3000000).ToList();
                        break;
                }
            }

            if (!string.IsNullOrEmpty(KinhNghiem))
            {
                switch (KinhNghiem)
                {
                    case "0":
                        data = data.Where(x => x.sKinhNghiem.Contains("0") || x.sKinhNghiem.Contains("Chưa")).ToList();
                        break;
                    case "1-2":
                        data = data.Where(x => x.sKinhNghiem.Contains("1") || x.sKinhNghiem.Contains("2")).ToList();
                        break;
                    case "3-5":
                        data = data.Where(x => x.sKinhNghiem.Contains("3") || x.sKinhNghiem.Contains("4") || x.sKinhNghiem.Contains("5")).ToList();
                        break;
                }
            }

            // Trả kết quả về ViewModel
            var ketqua = data.Select(x => new BaiDangViewModel
            {
                PK_iMaBaiDang = x.PK_iMaBaiDang,
                sTieuDe = x.sTieuDe,
                sMoTa = x.sMoTa,
                sDiaDiem = x.sDiaDiem,
                fMucLuong = x.fMucLuong ?? 0,
                sTrangThai = x.sTrangThai,
                dNgayTao = x.dNgayTao,
                dThoiGianHetHan = x.dThoiGianHetHan ?? DateTime.MinValue,
                Nguoitao = x.TaiKhoan.UserName,
                sMonday = x.sMonday,
                sKinhNghiem = x.sKinhNghiem,
                sTuoi = x.sTuoi,
                sBangCap = x.sBangCap,
                sGioiTinh = x.sGioiTinh,
                sfileAvata = x.TaiKhoan.FileAvata,
                Vaitro = x.TaiKhoan.VaiTro,
                FileCVPath = x.FileCVPath
            }).ToList();

            return ketqua;
        }

    }
}
