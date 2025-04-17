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
        private readonly IHoSoService _hoSoService;

        public AdminController(ILogger<AdminController> logger,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context,
            IConfiguration configuration,
            UserManager<TaiKhoan> userManager,
            SignInManager<TaiKhoan> signInManager,
            IDangTinService dangTinService,
            IHoSoService hoSoService
            )
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _dangTinService = dangTinService;
            _hoSoService = hoSoService;
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
        public async Task<IActionResult> DSHS(string trangthai = "Đang chờ duyệt")
        {
            var hoSoList = await _hoSoService.GetAllHoSoByTrangThai(trangthai);

            return View(hoSoList);

        }
        [HttpGet]
        public async Task<IActionResult> GetHS(int Primakey)
        {
            var hs = await _hoSoService.GetHoSoById(Primakey);
            if (hs == null)
            {
                return NotFound();
            }
            return View(hs);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteHS(int id)
        {
            var result = await _hoSoService.DeleteChangesAsync(id);
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
        public async Task<IActionResult> PheDuyetHS(int pd)
        {
            var result = await _hoSoService.PheDuyetHSAsync(pd);
            if (result)
            {
                TempData["Message"] = "Phê duyệt thành công";
            }
            else
            {
                TempData["Error"] = "Không thể phê duyệt";
            }
            return RedirectToAction("DSHS");
        }
        [HttpPost]
        public async Task<IActionResult> SearchHSA(string kynang, string kinhnghiem, string tieude, string trangthai)
        {
            // Nếu không có trạng thái thì mặc định là 'Đang chờ duyệt'

            trangthai = string.IsNullOrEmpty(trangthai) ? "Đang chờ duyệt" : trangthai;

            var HSList = new List<HoSoViewModel>();
            HSList = await _hoSoService.GetAllHoSoByTrangThai(trangthai);

            if (!string.IsNullOrEmpty(kynang))
            {
                HSList = HSList.Where(bd => bd.sKyNang.Contains(kynang)).ToList();
            }

            if (!string.IsNullOrEmpty(kinhnghiem))
            {
                HSList = HSList.Where(bd => bd.sKinhNghiem.Contains(kinhnghiem)).ToList();
            }
            if (!string.IsNullOrEmpty(tieude))
            {
                HSList = HSList.Where(bd => bd.sTieuDe.Contains(tieude)).ToList();
            }
            return View("Index", HSList);
        }
        [HttpGet]
        public async Task<IActionResult> MoCVA(int id)
        {
            var hs = await _hoSoService.GetHoSoById(id);
            if (hs == null)
            {
                return NotFound();
            }
            return View(hs);
        }
        public async Task<IActionResult> DanhSachUser()
        {
            var users = await _userManager.Users
                .Select(u => new TaiKhoanViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    PhoneNumber = u.PhoneNumber,
                    DiaChi = u.DiaChi,
                    VaiTro=u.VaiTro,
                    CCCD = u.CCCD

                }).ToListAsync();

            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteTK(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Xoá tài khoản thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Xoá tài khoản thất bại.";
            }

            return RedirectToAction("DanhSachUser");
        }
        [HttpPost]
        public async Task<IActionResult> SearchTK(string nguoidang)
        {

                    var TKList = await _context.Users
                   .Where(t => t.UserName == nguoidang)
                   .Select(t => new TaiKhoanViewModel
                   {
                       Id = t.Id,
                       UserName = t.UserName,
                       Email = t.Email,
                       PhoneNumber=t.PhoneNumber,
                       CCCD = t.CCCD,
                       VaiTro = t.VaiTro,
                       // Thêm các trường khác nếu cần
                   })
               .ToListAsync();
            return View("DanhSachUser", TKList);

        }


    }
}
