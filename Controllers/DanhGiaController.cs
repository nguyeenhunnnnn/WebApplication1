using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    public class DanhGiaController : Controller
    {
        private readonly IDanhGiaService _danhGiaService;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly ApplicationDbContext _context;
        public DanhGiaController(IDanhGiaService danhGiaService, UserManager<TaiKhoan> userManager, ApplicationDbContext context)
        {
            _danhGiaService = danhGiaService;
            _userManager = userManager;
            _context = context;
        }
        [HttpGet]
        public IActionResult TaoDanhGia(string giaSuId)
        {
            // Tạo ViewModel để truyền qua View nếu cần
            var viewModel = new DanhGiaViewModel
            {
                GiaSuId = giaSuId,
                // Có thể thêm các field khác tùy ý
            };

            return View(viewModel); // Trả về view hiển thị form đánh giá   
        }

        [HttpPost]
        public async Task<IActionResult> TaoDanhGia(string giaSuId, int soSao, string? noiDung)
        {
            var userId = _userManager.GetUserId(User);

            // Kiểm tra đã đánh giá chưa
            if (await _context.DanhGiaGiaSus.AnyAsync(d => d.NguoiDanhGiaId == userId && d.GiaSuId == giaSuId))
            {
                TempData["Tittle"] = "Bạn đã đánh giá gia sư này rồi.";
                TempData["ErrorMessage"] = "Đánh giá thất bại !";
                return View();
            }
            await _danhGiaService.AddDanhGiaAsync(userId!, giaSuId, soSao, noiDung);
            TempData["Tittle"] = "Đánh giá của bạn đã được gửi.";
            TempData["SuccessMessage"] = "Đánh giá thành công!";
            // return RedirectToAction("ChiTiet", "Profile", new { id = giaSuId });
            return RedirectToAction("DanhSachUngVien", "baiDang");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
