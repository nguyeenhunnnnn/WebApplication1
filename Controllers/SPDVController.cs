using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class SPDVController : Controller
    {
        private readonly ILogger<SPDVController> _logger;
        private readonly ApplicationDbContext _context;

        public SPDVController(ILogger<SPDVController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var goiDichVus = _context.GoiDichVus.ToList(); // hoặc dùng service gọi từ DB
            var model = new GoiViewModel
            {
                GoiDichVus = goiDichVus,
                BaiDangId = 1 // hoặc lấy ID hợp lệ tương ứng
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
