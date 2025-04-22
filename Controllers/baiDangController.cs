using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using WebApplication1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class baiDangController : Controller
    {
        private readonly ILogger<baiDangController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly IDangTinService _dangTinService;
        private readonly IDangTinRepository _dangTinRepository;

        public baiDangController(
            ILogger<baiDangController> logger,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context,
            IConfiguration configuration,
            UserManager<TaiKhoan> userManager,
            SignInManager<TaiKhoan> signInManager,
            IDangTinService dangTinService,
            IDangTinRepository dangTinRepository)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _dangTinService = dangTinService;
            _dangTinRepository = dangTinRepository;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult DangTin()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> DangTin(DangTinViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _logger.LogInformation("DangTin action called with model: {@Model}", model);


            if (string.IsNullOrEmpty(model.sTieuDe) || string.IsNullOrEmpty(model.sDiaDiem) || model.fMucLuong <= 0 || string.IsNullOrEmpty(model.sMoTa) || string.IsNullOrEmpty(model.sYCau) || string.IsNullOrEmpty(model.sBangCap) || string.IsNullOrEmpty(model.sGioiTinh) || string.IsNullOrEmpty(model.sKinhNghiem) || string.IsNullOrEmpty(model.sMonday) || string.IsNullOrEmpty(model.sTuoi))
            {
                ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin!");
                return View(model);
            }

            //var dangTin = new BaiDang();

          //  bool isExist = await _dangTinService.DangTinExistsByTitleAsync(model.sTieuDe);
            //if (isExist)
           // {
            //    ModelState.AddModelError("", "Tiêu đề đã tồn tại!");
           //     return View(model);
           // }
            /*  dangTin.sMonday = model.sMonday;
              dangTin.sYCau = model.sYCau;
              dangTin.sGioiTinh = model.sGioiTinh;
              dangTin.sTuoi = model.sTuoi;
              dangTin.sKinhNghiem = model.sKinhNghiem;
              dangTin.sBangCap = model.sBangCap;
              dangTin.sTieuDe = model.sTieuDe;
              dangTin.sDiaDiem = model.sDiaDiem;
              dangTin.fMucLuong = model.fMucLuong;
              dangTin.dThoiGianHetHan = model.dThoiGianHetHan;
              dangTin.sMoTa = model.sMoTa;
              dangTin.dNgayTao = DateTime.Now;
              dangTin.sTrangThai = "Đang chờ duyệt";
              dangTin.FK_iMaTK = userId;*/
            bool result = await _dangTinService.CreatDangTin(model, userId);
            if (result)
            {
                TempData["Success"] = "Đăng tin thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Đăng tin không thành công!");
            }
            return View(model);
        }

        
        public async Task<IActionResult> Index(string trangthai = "Đang chờ duyệt")
        {
           
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var baidangList = new List<BaiDangViewModel>();
            baidangList = await _dangTinService.GetAllBaiDangByTaiKhoanId(userId, trangthai);
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
        public async Task<IActionResult> SearchDB(string MonDay, string Diadiem, string ThoiGian, string TuKhoa,string nguoidang)
        { string trangthai = "Đang chờ duyệt";
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var baiDangList = new List<BaiDangViewModel>();
            baiDangList = await _dangTinService.GetAllBaiDangByTaiKhoanId(userId, trangthai);
            if (!string.IsNullOrEmpty(MonDay))
            {
                baiDangList = baiDangList.Where(bd => bd.sMonday.Contains(MonDay)).ToList();
            }
            if (!string.IsNullOrEmpty(nguoidang))
            {
                baiDangList = baiDangList.Where(bd => bd.Nguoitao.Contains(nguoidang)).ToList();
            }
            if (!string.IsNullOrEmpty(Diadiem))
            {
                baiDangList = baiDangList.Where(bd => bd.sDiaDiem.Contains(Diadiem)).ToList();
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
                baiDangList = baiDangList.Where(bd => bd.sMoTa.Contains(TuKhoa)).ToList();
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
        public async Task<IActionResult> Edit(int id)
        {
            var dangTin = await _dangTinService.GetBaiDangById(id);
            if (dangTin == null)
            {
                return NotFound();
            }
            var baidang = new BaiDangViewModel
            {
                PK_iMaBaiDang = dangTin.PK_iMaBaiDang,
                sTieuDe = dangTin.sTieuDe,
                sMoTa = dangTin.sMoTa,
                sDiaDiem = dangTin.sDiaDiem,
                fMucLuong = dangTin.fMucLuong,
                dThoiGianHetHan = dangTin.dThoiGianHetHan,
                dNgayTao = dangTin.dNgayTao,
                sTrangThai = dangTin.sTrangThai,
                sMonday = dangTin.sMonday,
                sYCau = dangTin.sYCau,
                sGioiTinh = dangTin.sGioiTinh,
                sTuoi = dangTin.sTuoi,
                sKinhNghiem = dangTin.sKinhNghiem,
                sBangCap = dangTin.sBangCap
            };
            return View(baidang);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(BaiDangViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (string.IsNullOrEmpty(model.sTieuDe) || string.IsNullOrEmpty(model.sDiaDiem) || model.fMucLuong <= 0 || string.IsNullOrEmpty(model.sMoTa) || string.IsNullOrEmpty(model.sYCau) || string.IsNullOrEmpty(model.sBangCap) || string.IsNullOrEmpty(model.sGioiTinh) || string.IsNullOrEmpty(model.sKinhNghiem) || string.IsNullOrEmpty(model.sMonday) || string.IsNullOrEmpty(model.sTuoi))
            {
                ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin!");
                return View(model);
            }
           

            try
            {
                bool result = await _dangTinService.UpdateDangTin(model);
                if (result)
                {
                    TempData["Success"] = "Cập nhật tin thành công!";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Cập nhật không thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật bài đăng.");
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật bài đăng.");
            }
            return View(model);


        }





    }   

    
}
