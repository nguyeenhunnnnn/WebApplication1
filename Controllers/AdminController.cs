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
using WebApplication1.Services;
using WebApplication1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly IDangTinService _dangTinService;

        public AdminController(ILogger<AdminController> logger,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context,
            IConfiguration configuration,
            UserManager<TaiKhoan> userManager,
            SignInManager<TaiKhoan> signInManager,
            IDangTinService dangTinService
            )
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _dangTinService = dangTinService;
        }
        public async Task<IActionResult> Index(string trangthai = "Đang chờ duyệt")
        {
           
            var baidangList = new List<BaiDangViewModel>();
            baidangList = await _dangTinService.GetAllBaiDangByTrangthai(trangthai);
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
        public async Task<IActionResult> SearchDBA( string ThoiGian, string TuKhoa, string nguoidang,string trangthai )
        {
            // Nếu không có trạng thái thì mặc định là 'Đang chờ duyệt'
            
            trangthai = string.IsNullOrEmpty(trangthai) ? "Đang chờ duyệt" : trangthai; 

            var baiDangList = new List<BaiDangViewModel>();
            baiDangList = await _dangTinService.GetAllBaiDangByTrangthai(trangthai);
           
            if (!string.IsNullOrEmpty(nguoidang))
            {
                baiDangList = baiDangList.Where(bd => bd.Nguoitao.Contains(nguoidang)).ToList();
            }
           
            if (!string.IsNullOrEmpty(ThoiGian))
            {
                DateTime dateTime;
                if (DateTime.TryParse(ThoiGian, out dateTime))
                {
                    baiDangList = baiDangList.Where(bd => bd.dNgayTao.Date == dateTime.Date).ToList();
                }
            }
            if (!string.IsNullOrEmpty(TuKhoa))
            {
                baiDangList = baiDangList.Where(bd => bd.sTieuDe.Contains(TuKhoa)).ToList();
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
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _dangTinService.DeleteDangTin(id);
            if (result)
            {
                TempData["Success"] = "Xóa tin thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Xóa tin không thành công!";
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<IActionResult> PheDuyet(int pd)
        {
            var result = await _dangTinService.PheDuyetBaiDangAsync(pd);
            if (result)
            {
                TempData["Message"] = "Phê duyệt thành công";
            }
            else
            {
                TempData["Error"] = "Không thể phê duyệt";
            }
            return RedirectToAction("Index");
        }

    }
}
