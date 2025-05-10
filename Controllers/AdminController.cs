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
using Microsoft.AspNetCore.Identity.UI.Services;

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
        private readonly IDangTinRepository _dangTinRepository;
        private readonly IHoSoService _hoSoService;
        private readonly ICustomEmailSender _emailSender;
        private readonly IGoiDichVuService _goiService;
        private readonly IThanhToanService _thanhToanService;
        private readonly IAccountService _accountService;

        public AdminController(ILogger<AdminController> logger,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context,
            IConfiguration configuration,
            UserManager<TaiKhoan> userManager,
            SignInManager<TaiKhoan> signInManager,
            IDangTinService dangTinService,
            IGoiDichVuService goiService,
             IThanhToanService thanhToanService,
             IAccountService accountService,
             IDangTinRepository dangTinRepository,
             ICustomEmailSender emailSender,
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
            _goiService = goiService;
            _thanhToanService = thanhToanService;
            _dangTinRepository = dangTinRepository;
            _emailSender = emailSender;
            _accountService = accountService;
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
        [HttpGet]
        public async Task<IActionResult> TuChoi(int pd)
        {
            var baiDang = await _dangTinService.GetDangTinById(pd);
            if (baiDang == null) return NotFound();

            var nguoiTao = await _accountService.GetTaiKhoanByIdAsync(baiDang.FK_iMaTK);
            if (nguoiTao == null) return NotFound();

            // Gửi mail
            string toEmail = "nguyeenhuong12345@gmail.com";
            string subject = "Bài đăng của bạn đã bị từ chối";
            string body = $"Xin chào {nguoiTao.UserName},\n\nBài đăng \"{baiDang.sTieuDe}\" của bạn đã bị từ chối với lý do:\n\"Cần chỉnh sửa lại bài đăng cho hợp lệ\".";

            await _emailSender.SendEmailAsync(toEmail, subject, body);

            // (Tuỳ chọn) Cập nhật trạng thái bài đăng nếu cần
            baiDang.sTrangThai = "Bị từ chối";
            await _dangTinRepository.UpdateDangTin(baiDang);

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

        // quan ly goi dich vu
        public IActionResult QuanLyGoi()
        {
            var list = _goiService.LayDanhSachGoi();
            return View(list);
        }

        public IActionResult CreateGDV()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateGDV(ChonGoiViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp
                if (_goiService.GoiDaTonTai(model.TenGoi, model.SoNgayHieuLuc))
                {
                    ModelState.AddModelError("", "❌ Không được trùng tên gói và số ngày.");
                    return View(model);
                }
                _goiService.AddAsync(model);
                ModelState.Clear();
                ViewBag.SuccessMessage = "✅ Đã thêm gói dịch vụ thành công!";
                return View(new ChonGoiViewModel());
                /*_goiService.AddAsync(model);
                return RedirectToAction("QuanLyGoi");*/
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage); // hoặc log ra file
            }
            return View(model);
        }

        public IActionResult EditGDV(int id)
        {
            var model = _goiService.LayGoiTheoId(id);
            if (model == null)
                return NotFound();
            return View(model);
        }

        [HttpGet]
        public IActionResult EditGDV(ChonGoiViewModel model)
        {
            if (ModelState.IsValid)
            {
                _goiService.UpdateAsync(model);
                return RedirectToAction("QuanLyGoi");
            }
            return View(model);
        }
        public IActionResult DeleteGDV(int id)
        {
            var model = _goiService.LayGoiTheoId(id);
            if (model == null)
                return NotFound();
            return View(model);
        }

        // POST: /GoiDichVu/Delete/5
        [HttpPost, ActionName("DeleteGDV")]
        public async Task<IActionResult> DeleteGDVConfirmed(int id)
        {
            await _goiService.DeleteAsync(id);
            return RedirectToAction("QuanLyGoi");
        }
        // quan ly giao dich
        public async Task<IActionResult> QuanLyGD(string trangthaiGD)
        {
            bool isDuyet = trangthaiGD == "Đã duyệt";
            var list = await _thanhToanService.GetByTrangThaiAsync(isDuyet);
            return View(list);
        }

        // duyet thanh toan
        [HttpPost]
        public async Task<IActionResult> Duyet(int id)
        {
            await _thanhToanService.DuyetThanhToanAsync(id);

            return RedirectToAction("QuanLyGD", new { trangthaiGD = "Đã duyệt" });
        }

    }
}
