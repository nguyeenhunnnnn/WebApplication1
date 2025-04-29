using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class DanhGiaController : Controller
    {
        private readonly IDanhGiaService _danhGiaService;
        private readonly UserManager<TaiKhoan> _userManager;

        public DanhGiaController(IDanhGiaService danhGiaService, UserManager<TaiKhoan> userManager)
        {
            _danhGiaService = danhGiaService;
            _userManager = userManager;
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
            await _danhGiaService.AddDanhGiaAsync(userId!, giaSuId, soSao, noiDung);
           // return RedirectToAction("ChiTiet", "Profile", new { id = giaSuId });
           return RedirectToAction("Index", "Home");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
