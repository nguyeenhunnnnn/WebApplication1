using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;
using WebApplication1.Repositories;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly IGoiDichVuRepository _goiRepo;
        private readonly IThanhToanRepository _thanhToanRepo;
        private readonly IThanhToanService _thanhToanService;
        private readonly IDangTinRepository _baiDangRepo;
        private readonly UserManager<TaiKhoan> _userManager;

        public ThanhToanController(
           IGoiDichVuRepository goiRepo,
           IThanhToanRepository thanhToanRepo,
           IThanhToanService thanhToanService,
           IDangTinRepository baiDangRepo,
           UserManager<TaiKhoan> userManager)
        {
            _goiRepo = goiRepo;
            _thanhToanRepo = thanhToanRepo;
            _thanhToanService = thanhToanService;
            _baiDangRepo = baiDangRepo;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ChonGoi(int baiDangId)
        {
            var gois = _goiRepo.GetAll();
            var vm = new GoiViewModel
            {
                BaiDangId = baiDangId,
                GoiDichVus = gois
            };
            return View(vm);
        }

        public IActionResult XacNhan(int goiId, int baiDangId)
        {
            var goi = _goiRepo.GetById(goiId);
            var vm = new XacNhanTTViewModel
            {
                BaiDangId = baiDangId,
                GoiDichVu = goi
            };
            if (goi == null)
            {
                TempData["Tittle"] = "Vui lòng chọn lại gói";
                TempData["ErrorMessage"] = "Gói dịch vụ không tồn tại.";
                return RedirectToAction("ChonGoi", new { BaiDangId = baiDangId });
            }
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ThanhToanGoi(int BaiDangId, int GoiId)
        {
            var user = await _userManager.GetUserAsync(User);
            var goi = _goiRepo.GetById(GoiId);

            var thanhToan = new ThanhToan
            {
                TaiKhoanId = user.Id,
                GoiDichVuId = GoiId,
                NgayThanhToan = DateTime.Now,
                BaiDangId = BaiDangId,
                IsDuyet = false,
            };

            _thanhToanRepo.Them(thanhToan);
            var baiDang = await _baiDangRepo.GetBaiDangById(BaiDangId);
            if (baiDang != null)
            {
                baiDang.sTrangThaiGD = "Chờ duyệt"; // bạn nên dùng enum hoặc hằng số nếu có
                await _baiDangRepo.UpdateDangTin(baiDang);
                TempData["Tittle"] = "Thanh toán của bạn đang chờ duyệt";
                TempData["SuccessMessage"] = "Thanh toán thành công!";
                return RedirectToAction("Index", "baiDang");
            }
            else
            {
                TempData["Tittle"] = "Vui lòng thanh toán lại.";
                TempData["ErrorMessage"] = "Thanh toán thất bại !";
                return View();
            }
                // Đẩy bài đăng lên đầu (cập nhật NgayDang)
                //_baiDangRepo.CapNhatThoiGianDang(BaiDangId, DateTime.Now.AddDays(goi.SoNgayHieuLuc));

                return RedirectToAction("Index", "baiDang");
        }

        public async Task<IActionResult> DonHang()
        {
            var userId = _userManager.GetUserId(User);
            var model = await _thanhToanService.GetLichSuThanhToanAsync(userId);
            return View(model);
        }

        /*[Authorize(Roles = "Admin")]
        public async Task<IActionResult> LichSuThanhToanAdmin()
        {
            var model = await _thanhToanService.GetAllLichSuThanhToanAsync();
            return View(model);
        }*/



    }
}
