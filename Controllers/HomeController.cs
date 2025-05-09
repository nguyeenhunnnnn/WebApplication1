using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IDanhGiaService _danhGiaService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IDanhGiaService danhGiaService)
        {
            _logger = logger;
            _context = context;
            _danhGiaService = danhGiaService;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var users = _context.Users.ToList();
            var hoSos = _context.HoSos.ToList();
            var result = new List<ProfileViewModel>();
            foreach (var user in users)
            {
                var hoSo = hoSos.FirstOrDefault(h => h.FK_iMaTK == user.Id);

                // L?y danh s�ch ?�nh gi� t? service (b?t ??ng b?)
                var danhGias = await _danhGiaService.GetDanhGiaByGiaSuIdAsync(user.Id);

                result.Add(new ProfileViewModel
                {
                    VaiTro = user.VaiTro,
                    Email = user.Email,
                    HoTen = user.UserName,
                    sFile_Avata_Path = user.FileAvata,
                    // hien thi goi cuoc
                    GoiCuoc = user.GoiCuoc,
                    // hien thị thông tin
                    TieuDeCV = hoSo?.sTieuDe ?? "",
                    // hien thi danh gia
                    DanhGias = danhGias
                });
            }

            return View(result);
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
