using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;
using WebApplication1.Repositories;
using WebApplication1.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Security.Claims;


namespace WebApplication1.Controllers
{
    public class BangTinController : Controller
    {
        private readonly IBangTinService _bangTinService;
        private readonly IBangTinRepository _bangTinRepository;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly IUngTuyenService _IUngTuyenService;
        private readonly IAccountRepository _taiKhoanRepo;

        public BangTinController(IBangTinService bangTinService, IBangTinRepository bangTinRepository, UserManager<TaiKhoan> userManager,IUngTuyenService ungTuyenService, IAccountRepository taiKhoanRepo)
        {
            _bangTinService = bangTinService;
            _bangTinRepository = bangTinRepository;
            _userManager = userManager;
            _IUngTuyenService = ungTuyenService;
            _taiKhoanRepo = taiKhoanRepo;
        }
        public IActionResult Index()
        { var page = new BaiDangPageViewModel();
            var userId = _userManager.GetUserId(User); // sẽ là null nếu chưa đăng nhập
            page = _bangTinService.GetAllBaiDangsAsync(userId).Result;
            if (page == null)
            {
                return NotFound();
            }
            return View(page);
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

            try
            {
                TempData["Tittle"] = "Hồ sơ của bạn đang chờ chấp nhận";
                TempData["SuccessMessage"] = "Ứng tuyển thành công!";
                await _IUngTuyenService.UngTuyenAsync(userId, baiDangId);
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                // Nếu gia sư đã ứng tuyển vào bài đăng
                TempData["Tittle"] = "Bạn đã ứng tuyển bài đăng này";
                TempData["ErrorMessage"] = "Ứng tuyển thất bại!";
                return View("Index"); // Quay lại trang Index với thông báo lỗi
            }
        }
      




    }
}
