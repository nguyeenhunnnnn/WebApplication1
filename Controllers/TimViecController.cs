using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using WebApplication1.Models.Entities;
using System.Security.Claims;
using WebApplication1.Repositories;

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

        public TimViecController(
            ILogger<TimViecController> logger,
            IAccountService accountService,
            IAccountRepository taiKhoanRepo,
            IDangTinService dangTinService,
            UserManager<TaiKhoan> userManager,
            SignInManager<TaiKhoan> signInManager)
        {
            _logger = logger;
            _accountService = accountService;
            _taiKhoanRepo = taiKhoanRepo;
            _dangTinService = dangTinService;
            _userManager = userManager;
            _signInManager = signInManager;
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
    }
}
