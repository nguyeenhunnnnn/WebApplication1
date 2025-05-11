using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using WebApplication1.Models.Entities;
using System.Security.Claims;
using WebApplication1.Repositories;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.ViewModels;
using System;

namespace WebApplication1.Controllers
{

    public class TimViecController : Controller
    {
        private readonly ILogger<TimViecController> _logger;
        private readonly IAccountService _accountService;
        private readonly IAccountRepository _taiKhoanRepo;
        private readonly IDangTinService _dangTinService;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IUngTuyenRepository _repo;
        private readonly IUngTuyenService _IUngTuyenService;

        public TimViecController(
            ILogger<TimViecController> logger,
            IAccountService accountService,
            IAccountRepository taiKhoanRepo,
            IDangTinService dangTinService,
            UserManager<TaiKhoan> userManager,
            ApplicationDbContext context,
            IUngTuyenService IUngTuyenService,
            IUngTuyenRepository repo,
            SignInManager<TaiKhoan> signInManager)
        {
            _logger = logger;
            _accountService = accountService;
            _taiKhoanRepo = taiKhoanRepo;
            _dangTinService = dangTinService;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _IUngTuyenService = IUngTuyenService;
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var allBaiDangs = await _dangTinService.GetAllBaiDangByVaiTroPhuHuynh();
           

            // Nếu chưa đăng nhập
            if (string.IsNullOrEmpty(userId))
            {
                return View(allBaiDangs);
            }

            var taiKhoan = await _taiKhoanRepo.GetTaiKhoanByIdAsync(userId);
            if (taiKhoan == null || taiKhoan.VaiTro != "giasu")
                return Forbid();

            var dsMonDayCuaGiaSu = new List<string>();

            if (User.Identity.IsAuthenticated && taiKhoan.VaiTro == "giasu")
            {
                // Ví dụ: lấy tất cả môn dạy của gia sư từ bảng Bài đăng của họ
                var monDay = _context.BaiDangs
                    .Where(x => x.FK_iMaTK == userId) // hoặc userId nếu có
                    .Select(x => x.sMonday)
                    .Distinct()
                    .ToList();

                dsMonDayCuaGiaSu = monDay;
            }

            ViewBag.MonGiaSuDay = dsMonDayCuaGiaSu;
            var baiDangGiaSu = await _dangTinService.GetAllBaiDangByTaiKhoanId(userId, "Đã duyệt");

            if (baiDangGiaSu.Any())
            {
                var tieuDeGiaSu = baiDangGiaSu.First().sTieuDe?.Trim().ToLower();

                var sorted = allBaiDangs
                    .OrderByDescending(b => b.sTieuDe?.Trim().ToLower() == tieuDeGiaSu)
                    .ThenByDescending(b => b.dThoiGianHetHan > DateTime.Now)                     // Ưu tiên bài còn hạn
                     .ThenBy(b => b.dThoiGianHetHan)                                     // Gần hết hạn hơn nằm trên
                    .ToList();

                return View(sorted);
            }

            return View(allBaiDangs);
        }


        [HttpGet]
        public async Task<IActionResult> Search(string Keyword,string MonHoc,string diadiem, string MucLuong, string KinhNghiem)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var taiKhoan = await _taiKhoanRepo.GetTaiKhoanByIdAsync(userId);
            if (taiKhoan == null)
                return Unauthorized();
            if (taiKhoan.VaiTro != "giasu")
                return Forbid();
            var baiDangs = await _dangTinService.TimKiemBaiDangAsync(Keyword, MonHoc, diadiem, MucLuong, KinhNghiem);
            return View("Index", baiDangs);
        }

        public async Task<IActionResult> SearchMH(string monhoc)
        {
            var dsBaiDang = await _dangTinService.SearchBaiDangsAsync(monhoc);
            ViewBag.MonHoc = monhoc;
            return View("Index", dsBaiDang);
        }
        [HttpPost]
        public async Task<IActionResult> UngTuyen(int baiDangId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var taiKhoan = await _taiKhoanRepo.GetTaiKhoanByIdAsync(userId);

            if (taiKhoan == null)
                return Unauthorized();

            if (taiKhoan.VaiTro != "giasu")
                return Forbid(); // 🚫 Không cho phép nếu không phải Gia Sư
                                 // Kiểm tra nếu gia sư đã ứng tuyển vào bài đăng này
            var daUngTuyen = await _repo.CheckGiaSuHasAppliedAsync(userId, baiDangId);

            if (daUngTuyen)
            {
                TempData["Tittle"] = "Ứng tuyển thất bại!";
                TempData["ErrorMessage"] = "Bạn đã ứng tuyển vào bài đăng này.";
                return RedirectToAction("Index");

            }
            await _IUngTuyenService.UngTuyenAsync(userId, baiDangId);
            TempData["Tittle"] = "Ứng tuyển thành công.";
            TempData["SuccessMessage"] = "Hồ sơ của bạn đang chờ chấp nhận.";
            return RedirectToAction("Index");
 
        }

    }
}
