using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using WebApplication1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Globalization;
using System.Text;

namespace WebApplication1.Controllers
{
    public class baiDangController : Controller
    {
        private readonly ILogger<baiDangController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly IDangTinService _dangTinService;
        private readonly IDangTinRepository _dangTinRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUngTuyenService _IUngTuyenService;
        private readonly ICustomEmailSender _emailSender;
        private readonly IUngTuyenRepository _ungTuyenRepo;

        public baiDangController(
            ILogger<baiDangController> logger,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context,
            IConfiguration configuration,
            UserManager<TaiKhoan> userManager,
            SignInManager<TaiKhoan> signInManager,
            IDangTinService dangTinService,
            IAccountRepository accountRepository,
            IUngTuyenService IUngTuyenService,
            IDangTinRepository dangTinRepository,
            ICustomEmailSender emailSender,
            IUngTuyenRepository ungTuyenRepo
            )
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _dangTinService = dangTinService;
            _dangTinRepository = dangTinRepository;
            _accountRepository = accountRepository;
            _IUngTuyenService = IUngTuyenService;
            _emailSender = emailSender;
            _ungTuyenRepo = ungTuyenRepo;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult DangTin()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> DangTin(DangTinViewModel model)
        {
           
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _logger.LogInformation("DangTin action called with model: {@Model}", model);


            if (string.IsNullOrEmpty(model.sTieuDe) || string.IsNullOrEmpty(model.sDiaDiem) || model.fMucLuong <= 0 || string.IsNullOrEmpty(model.sMoTa) || string.IsNullOrEmpty(model.sYCau) || string.IsNullOrEmpty(model.sBangCap) || string.IsNullOrEmpty(model.sGioiTinh) || string.IsNullOrEmpty(model.sKinhNghiem) || string.IsNullOrEmpty(model.sMonday) || string.IsNullOrEmpty(model.sTuoi))
            {
                ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin!");
                return View(model);
            }

            //var dangTin = new BaiDang();

          //  bool isExist = await _dangTinService.DangTinExistsByTitleAsync(model.sTieuDe);
            //if (isExist)
           // {
            //    ModelState.AddModelError("", "Tiêu đề đã tồn tại!");
           //     return View(model);
           // }
            /*  dangTin.sMonday = model.sMonday;
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
              dangTin.FK_iMaTK = userId;*/
           
            bool result = await _dangTinService.CreatDangTin(model, userId);
            if (result)
            {
                TempData["Tittle"] = "Bài đăng của bạn đang chờ duyệt";
                TempData["SuccessMessage"] = "Đăng tin thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Tittle"] = "Vui lòng điền đầy đủ thông tin";
                TempData["ErrorMessage"] = "Đăng tin thất bại!";
                //ModelState.AddModelError("", "Đăng tin không thành công!");
            }
            return View(model);
        }

        
        public async Task<IActionResult> Index(string trangthai = "Đang chờ duyệt")
        {
           
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var baidangList = new List<BaiDangViewModel>();
            baidangList = await _dangTinService.GetAllBaiDangByTaiKhoanId(userId, trangthai);
            /* var baiDangList=_context.BaiDangs
                 .Where(bd => bd.FK_iMaTK ==userId)
                 .ToList();
             var baidangList = new List<BaiDangViewModel>();
             baidangList=baiDangList.Select(bd => new BaiDangViewModel()
             {

                 sTieuDe = bd.sTieuDe,
                 sMoTa = bd.sMoTa,
                 sDiaDiem = bd.sDiaDiem,
                 fMucLuong = bd.fMucLuong ?? 0,
                 dThoiGianHetHan = bd.dThoiGianHetHan ?? DateTime.MinValue,
                 dNgayTao = bd.dNgayTao,
                 sTrangThai = bd.sTrangThai,

             }).ToList();*/

            return View(baidangList);
        }
        public async Task<IActionResult> IndexGS(string trangthai = "Đang chờ duyệt")
        {

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var baidangList = new List<BaiDangViewModel>();
            baidangList = await _dangTinService.GetAllBaiDangByTaiKhoanId(userId, trangthai);
            /* var baiDangList=_context.BaiDangs
                 .Where(bd => bd.FK_iMaTK ==userId)
                 .ToList();
             var baidangList = new List<BaiDangViewModel>();
             baidangList=baiDangList.Select(bd => new BaiDangViewModel()
             {

                 sTieuDe = bd.sTieuDe,
                 sMoTa = bd.sMoTa,
                 sDiaDiem = bd.sDiaDiem,
                 fMucLuong = bd.fMucLuong ?? 0,
                 dThoiGianHetHan = bd.dThoiGianHetHan ?? DateTime.MinValue,
                 dNgayTao = bd.dNgayTao,
                 sTrangThai = bd.sTrangThai,

             }).ToList();*/

            return View(baidangList);
        }
        [HttpPost]
        public async Task<IActionResult> SearchDB(string MonDay, string Diadiem, string ThoiGian, string TuKhoa,string nguoidang)
        { string trangthai = "Đang chờ duyệt";
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var baiDangList = new List<BaiDangViewModel>();
            baiDangList = await _dangTinService.GetAllBaiDangByTaiKhoanId(userId, trangthai);
            if (!string.IsNullOrEmpty(MonDay))
            {
                baiDangList = baiDangList.Where(bd => bd.sMonday.Contains(MonDay)).ToList();
            }
            if (!string.IsNullOrEmpty(nguoidang))
            {
                baiDangList = baiDangList.Where(bd => bd.Nguoitao.Contains(nguoidang)).ToList();
            }
            if (!string.IsNullOrEmpty(Diadiem))
            {
                baiDangList = baiDangList.Where(bd => bd.sDiaDiem.Contains(Diadiem)).ToList();
            }
            if (!string.IsNullOrEmpty(ThoiGian))
            {
                DateTime dateTime;
                if (DateTime.TryParse(ThoiGian, out dateTime))
                {
                    baiDangList = baiDangList.Where(bd => bd.dNgayTao.Date == dateTime.Date).ToList();
                }
            }
            if (!string.IsNullOrWhiteSpace(TuKhoa))
            {
                var keyword = RemoveDiacritics(TuKhoa).ToLower();
                baiDangList = baiDangList.Where(u => RemoveDiacritics(u.sMoTa).ToLower().Contains(keyword)).ToList();
            }
            return View("Index", baiDangList);
        }
        
        
      
        [HttpGet]
        public async Task<IActionResult> Get(int Primakey)
        {
            
            var dangTin = await _dangTinService.GetBaiDangById(Primakey);
            if (dangTin == null)
            {
                return NotFound();
            }
            
            return View(dangTin);
        }
        [HttpGet]
        public async Task<IActionResult> GetGS(int Primakey)
        {

            var dangTin = await _dangTinService.GetBaiDangByIdGS(Primakey);
            if (dangTin == null)
            {
                return NotFound();
            }

            return View(dangTin);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _dangTinService.DeleteDangTin(id);
            if (result)
            {
                TempData["Tittle"] = "Bài đăng của bạn đã xoá";
                TempData["SuccessMessage"] = "Xoá tin thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Tittle"] = "Bài đăng của bạn chưa xoá";
                TempData["ErrorMessage"] = "Xoá tin thất bại!";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dangTin = await _dangTinService.GetBaiDangById(id);
            if (dangTin == null)
            {
                return NotFound();
            }
            var baidang = new BaiDangViewModel
            {
                PK_iMaBaiDang = dangTin.PK_iMaBaiDang,
                sTieuDe = dangTin.sTieuDe,
                sMoTa = dangTin.sMoTa,
                sDiaDiem = dangTin.sDiaDiem,
                fMucLuong = dangTin.fMucLuong,
                dThoiGianHetHan = dangTin.dThoiGianHetHan,
                dNgayTao = dangTin.dNgayTao,
                sTrangThai = dangTin.sTrangThai,
                sMonday = dangTin.sMonday,
                sYCau = dangTin.sYCau,
                sGioiTinh = dangTin.sGioiTinh,
                sTuoi = dangTin.sTuoi,
                sKinhNghiem = dangTin.sKinhNghiem,
                sBangCap = dangTin.sBangCap
            };
            return View(baidang);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(BaiDangViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (string.IsNullOrEmpty(model.sTieuDe) || string.IsNullOrEmpty(model.sDiaDiem) || model.fMucLuong <= 0 || string.IsNullOrEmpty(model.sMoTa) || string.IsNullOrEmpty(model.sYCau) || string.IsNullOrEmpty(model.sBangCap) || string.IsNullOrEmpty(model.sGioiTinh) || string.IsNullOrEmpty(model.sKinhNghiem) || string.IsNullOrEmpty(model.sMonday) || string.IsNullOrEmpty(model.sTuoi))
            {
                ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin!");
                return View(model);
            }
           

            try
            {
                bool result = await _dangTinService.UpdateDangTin(model);
                if (result)
                {
                    TempData["Tittle"] = "Bài đăng của bạn đã được cập nhật";
                    TempData["SuccessMessage"] = "Cập nhật tin thành công!";
                    return RedirectToAction("Index");
                }
                TempData["Tittle"] = "Bài đăng của bạn không được cập nhật cập nhật";
                TempData["ErrorMessage"] = "Cập nhật tin thất bại!";
                ModelState.AddModelError("", "Cập nhật không thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật bài đăng.");
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật bài đăng.");
            }
            return View(model);


        }
        public async Task<IActionResult> DanhSachUngVien(string trangthai = "Chờ duyệt")
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var taiKhoan = await _accountRepository.GetTaiKhoanByIdAsync(userId);
            if (taiKhoan == null)
                return Unauthorized();
            if (taiKhoan.VaiTro != "phuhuynh")
                return Forbid(); // 🚫 Không cho phép nếu không phải Gia Sư
            var ds = await _IUngTuyenService.LayDanhSachUngVienCuaPhuHuynh(userId, trangthai);
            return View(ds);
        }
        [HttpGet]
        public async Task<IActionResult> Duyet(int id,int baiDangId, string trangThai)
        {
            var email= "nguyeenhuong12345@gmail.com";
            var ungVien = await _ungTuyenRepo.GetUngTuyenByIdAsync(id);
            if (ungVien == null) return NotFound();

            // Cập nhật trạng thái người được chọn
            await _IUngTuyenService.DuyetUngVien(id, trangThai);

            if (trangThai == "Chấp nhận")
            {
                // Gửi mail chấp nhận
                await _emailSender.SendEmailAsync(
                   // ungVien.TaiKhoanGiaSu.Email,
                   email,
                    "Chúc mừng bạn đã được nhận!",
                    "Bạn đã được chấp nhận làm gia sư cho bài đăng này."
                );
                
                // Từ chối các gia sư còn lại
                var ungViens = await _ungTuyenRepo.GetUngTuyenByBaiDangId(baiDangId);
                foreach (var uv in ungViens)
                {
                    if (uv.Id != id && uv.TrangThai != "Từ chối")
                    {
                        await _IUngTuyenService.DuyetUngVien(uv.Id, "Từ chối");

                        await _emailSender.SendEmailAsync(
                           // uv.TaiKhoanGiaSu.Email,
                           email,
                            "Kết quả ứng tuyển",
                            "Rất tiếc, bạn không được chọn làm gia sư cho bài đăng này."
                        );
                    }
                }
            }
            else if (trangThai == "Từ chối")
            {
                await _emailSender.SendEmailAsync(
                    //ungVien.TaiKhoanGiaSu.Email,
                    email,
                    "Ứng tuyển không thành công",
                    "Rất tiếc, bạn đã bị từ chối trong quá trình ứng tuyển."
                );
            }

            return RedirectToAction("DanhSachUngVien");
        }
        // GET: DangTin/Create
        public IActionResult CreateGS()
        {
            return View();
        }

        // POST: DangTin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGS(DangTinGiaSuViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Lấy userId từ claims

            
                var result = await _dangTinService.CreateDangTinForGiaSuAsync(model, userId);

                if (result)
                {
                TempData["Tittle"] = "Bài đăng của bạn đang chờ duyệt";
                TempData["SuccessMessage"] = "Đăng tin thành công!";
                return RedirectToAction("IndexGS");  // Chuyển hướng tới trang chính sau khi đăng bài
                }
                else
                {
                TempData["Tittle"] = "Bạn cần tạo hồ sơ và upload CV trước khi đăng bài";
                TempData["ErrorMessage"] = "Đăng bài thất bại !";
                ModelState.AddModelError("", "Bạn cần tạo hồ sơ và upload CV trước khi đăng bài.");
                }
            

            return View(model);
        }


        // Ẩn bài đăng
        [HttpPost]
        public async Task<IActionResult> HideBaiDang(int baiDangId, bool Ishidden)
        {
            await _dangTinService.HideBaiDang(baiDangId,Ishidden);
            return RedirectToAction("DanhSachUngVien");
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
