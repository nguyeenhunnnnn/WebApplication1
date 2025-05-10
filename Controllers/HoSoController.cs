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
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Globalization;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace WebApplication1.Controllers
{
    public class HoSoController : Controller
    {
        private readonly ILogger<HoSoController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly IHoSoService _hoSoService;
        private readonly IUngTuyenService _ungTuyenService;
        private readonly IAccountRepository _accountRepository;

        public HoSoController(ILogger<HoSoController> logger,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context,
            IConfiguration configuration,
            UserManager<TaiKhoan> userManager,
            SignInManager<TaiKhoan> signInManager,
            IHoSoService hoSoService,
            IUngTuyenService ungTuyenService,
            IAccountRepository accountRepository

            )
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _hoSoService = hoSoService;
            _ungTuyenService = ungTuyenService;
            _accountRepository = accountRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string trangthai = "Đang chờ duyệt")
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var hoSoList = await _hoSoService.GetAllHoSoByTaiKhoanId(userId,trangthai);

            return View(hoSoList);

        }
        [HttpGet]
        public async Task<IActionResult> CreateHoSo()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new HoSoViewModel
            {
                FK_iMaTK = user.Id,
                HoTen = user.UserName,
                SoDienThoai = user.PhoneNumber,
                Email = user.Email,
                DiaChi = user.DiaChi
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateHoSo(HoSoViewModel model)
        {
            // Kiểm tra xem model có hợp lệ không

            // Lấy thông tin người dùng
            var user = await _userManager.GetUserAsync(User);

            // Xử lý tệp tải lên
            string filePath = null;
            if (model.formFile != null && model.formFile.Length > 0 )
            {
                // Kiểm tra kích thước tệp (giới hạn 5MB)
                if (model.formFile.Length > 5 * 1024 * 1024 )
                {
                    ModelState.AddModelError("", "Tệp quá lớn. Vui lòng chọn tệp nhỏ hơn 5MB.");
                    return View(model);
                }
                 filePath = model.formFile != null
                    ? await UploadFile(model.formFile, "hoso")
                    : null;

            }
            model.sDuongDanTep = filePath;
            string filePathBC = null;
            if (model.formAnhBC != null && model.formAnhBC.Length > 0)
            {
                // Kiểm tra kích thước tệp (giới hạn 5MB)
                if (model.formAnhBC != null && model.formAnhBC.Length > 0)
                {
                    // Kiểm tra kích thước tệp (giới hạn 5MB)
                    if (model.formAnhBC.Length > 5 * 1024 * 1024)
                    {
                        TempData["Tittle"] = "Tệp quá lớn. Vui lòng chọn tệp nhỏ hơn 5MB.";
                        TempData["ErrorMessage"] = " Vui lòng chọn lại tệp!";
                        ModelState.AddModelError("", "Tệp quá lớn. Vui lòng chọn tệp nhỏ hơn 5MB.");
                        return View(model);
                    }
                    filePathBC = model.formAnhBC != null
                    ? await UploadFile(model.formAnhBC, "bangcap")
                    : null;
                }

            }
            model.sDuongDanTepBC = filePathBC;
            string trangthai = "Đang chờ duyệt";
            model.sTrangThai = trangthai;
            /*
            // Tạo đối tượng hồ sơ mới
            var hoSo = new HoSo
            {
                FK_iMaTK = user.Id,
                sKinhNghiem = model.sKinhNghiem,
                sBangCap = model.sBangCap,
                sKyNang = model.sKyNang,
                sTieuDe=model.sTieuDe,
                sDuongDanTep = model.sDuongDanTep // Lưu đường dẫn tệp vào cơ sở dữ liệu
            };

            // Lưu thông tin hồ sơ vào cơ sở dữ liệu
            await _context.HoSos.AddAsync(hoSo);
            await _context.SaveChangesAsync();

            // Sau khi lưu thành công, chuyển hướng người dùng tới trang khác (ví dụ trang hồ sơ)*/
            //Lưu thông tin hồ sơ
            bool result = await _hoSoService.CreateHoSoAsync(model, user.Id);
            if (result)
            {
                TempData["Tittle"] = "Hồ sơ của bạn đang chờ duyệt";
                TempData["SuccessMessage"] = "Tạo hồ sơ thành công!";
                return RedirectToAction("Index");// chuyển hướng trang danh sách 
            }
            else
            {
                TempData["Tittle"] = "Hồ sơ của bạn chưa đủ thông tin";
                TempData["ErrorMessage"] = "Tạo hồ sơ thất bại!";
                ModelState.AddModelError("", "Đăng tin không thành công!");
            }

            return View(model);
        }

        // Nếu model không hợp lệ, trả về view với thông báo lỗi



        [HttpGet]
        public async Task<IActionResult> Get(int Primakey)
        {
            var hs = await _hoSoService.GetHoSoById(Primakey);
            if (hs == null)
            {
                return NotFound();
            }
            return View(hs);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result=await _hoSoService.DeleteChangesAsync(id);
            if (result)
            {
                TempData["Tittle"] = "Hồ sơ của bạn đã xoá";
                TempData["SuccessMessage"] = "Xoá hồ sơ thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Tittle"] = "Hồ sơ của bạn chưa bị xoá";
                TempData["ErrorMessage"] = "Xoá hồ sơ thất bại!";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var hoSo = await _hoSoService.GetHoSoById(id);
            if (hoSo == null)
            {
                return NotFound();
            }
            var HosoInfo = new HoSoViewModel
            {
                iMaHS = hoSo.iMaHS,
                FK_iMaTK = hoSo.FK_iMaTK,
                sKinhNghiem = hoSo.sKinhNghiem,
                sBangCap = hoSo.sBangCap,
                sKyNang = hoSo.sKyNang,
                sTrangThai =hoSo.sTrangThai,
                sTieuDe = hoSo.sTieuDe,
                sDuongDanTep = hoSo.sDuongDanTep,
                sDuongDanTepBC = hoSo.sDuongDanTepBC,
                HoTen = hoSo.HoTen, // nếu có liên kết navigation property
                SoDienThoai = hoSo.SoDienThoai,
                Email = hoSo.Email,
                DiaChi = hoSo.DiaChi
            };
            return View(HosoInfo);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(HoSoViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Xử lý tệp tải lên
            string filePath = null;
            if (model.formFile != null && model.formFile.Length > 0)
            {
                // Kiểm tra kích thước tệp (giới hạn 5MB)
                if (model.formFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "Tệp quá lớn. Vui lòng chọn tệp nhỏ hơn 5MB.");
                    return View(model);
                }
                filePath = model.formFile != null
                   ? await UploadFile(model.formFile, "hoso")
                   : null;

                model.sDuongDanTep = filePath;
            }
            
            string filePathBC = null;
           
                // Kiểm tra kích thước tệp (giới hạn 5MB)
                if (model.formAnhBC != null && model.formAnhBC.Length > 0)
                {
                    // Kiểm tra kích thước tệp (giới hạn 5MB)
                    if (model.formAnhBC.Length > 5 * 1024 * 1024)
                    {
                        TempData["Tittle"] = "Tệp quá lớn. Vui lòng chọn tệp nhỏ hơn 5MB.";
                        TempData["ErrorMessage"] = " Vui lòng chọn lại tệp!";
                        ModelState.AddModelError("", "Tệp quá lớn. Vui lòng chọn tệp nhỏ hơn 5MB.");
                        return View(model);
                    }
                    filePathBC = model.formAnhBC != null
                    ? await UploadFile(model.formAnhBC, "bangcap")
                    : null;
                    model.sDuongDanTepBC = filePathBC;
                }

            
            

            try
            {
                bool result = await _hoSoService.UpdateHoSoAsync(model);
                if (result)
                {
                    TempData["Tittle"] = "Hồ sơ của bạn đã được cập nhật";
                    TempData["SuccessMessage"] = "Cập nhật Hồ sơ thành công!";
                    return RedirectToAction("Index");
                }
                TempData["Tittle"] = "Hồ sơ của bạn không được cập nhật cập nhật";
                TempData["ErrorMessage"] = "Cập nhật Hồ sơ thất bại!";
                ModelState.AddModelError("", "Cập nhật không thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật bài đăng.");
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật bài đăng.");
            }
            return View(model);


        }
        [HttpPost]
        public async Task<IActionResult> SearchHS(string kynang,string tieude, string hocvan, string kinhnghiem, string trangthai)
        {
            trangthai = string.IsNullOrEmpty(trangthai) ? "Đang chờ duyệt" : trangthai;
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var hoSoList = await _hoSoService.GetAllHoSoByTaiKhoanId(userId, trangthai);
            if (!string.IsNullOrEmpty(kynang))
            {
                hoSoList = hoSoList.Where(bd => bd.sKyNang.Contains(kynang)).ToList();
            }
           
            if (!string.IsNullOrEmpty(hocvan))
            {
               
                    hoSoList = hoSoList.Where(bd => bd.sBangCap.Contains(hocvan)).ToList();
                
            }
            if (!string.IsNullOrEmpty(kinhnghiem))
            {
                hoSoList = hoSoList.Where(bd => bd.sKinhNghiem.Contains(kinhnghiem)).ToList();
            }

           
            if (!string.IsNullOrWhiteSpace(tieude))
            {
                var keyword = RemoveDiacritics(tieude).ToLower();
                hoSoList = hoSoList.Where(u => RemoveDiacritics(u.sTieuDe).ToLower().Contains(keyword)).ToList();
            }
            
            return View("Index", hoSoList);

        }
        private async Task<string> UploadFile(IFormFile file, string folderName)
        {
            //kiem tra xem file có tồn tại hay không 
            if (file == null || file.Length == 0)
                return null;
            //tạo đường dẫn thư mục 
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folderName);
            //kiểm tra xem đường dẫn có tồn tại hay không 
            if (!Directory.Exists(uploadsFolder))
                //nếu không tồn tại tạo thư mục mới 
                Directory.CreateDirectory(uploadsFolder);
            //tạo tên file riêng , tránh trùng lặp 
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            //kết nối, tạo đường dẫn 
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            //lưu file vào trong đường dẫn đấy 
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            _logger.LogInformation($"Uploaded file to: {filePath}");
            // trả lại đường dẫn /uploads/cccd/avd.png 
            return Path.Combine("/uploads", folderName, uniqueFileName);
        }
        [HttpGet]
        public async Task<IActionResult> MoCV(int id)
        {
            var hs = await _hoSoService.GetHoSoById(id);
            if (hs == null)
            {
                return NotFound();
            }
            return View(hs);
        }
        public async Task<IActionResult> QuanLyUngTuyen(string trangthai = "Chờ duyệt")
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var taiKhoan = await _accountRepository.GetTaiKhoanByIdAsync(userId);
            if (taiKhoan == null)
                return Unauthorized();
            if (taiKhoan.VaiTro != "giasu")
                return Forbid(); // 🚫 Không cho phép nếu không phải Gia Sư
            var list = await _ungTuyenService.LayDanhSachUngTuyenCuaGiaSu(userId,trangthai);
            return View(list);
        }
        [HttpPost]
        public async Task<IActionResult> HuyUngTuyen(int baiDangId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                await _ungTuyenService.HuyUngTuyenAsync(userId, baiDangId);
                TempData["Tittle"] = "Đã hủy ứng tuyển";
                TempData["SuccessMessage"] = "Bạn đã hủy ứng tuyển thành công.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Tittle"] = "Bạn đã hủy ứng tuyển thất bại.";
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("QuanLyUngTuyen");
        }

        [HttpPost]
        public async Task<IActionResult> SearchUT(string tieude, DateTime? thoiGian)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var ketQua = await _ungTuyenService.TimKiemHoSoAsync(tieude, thoiGian, userId);
            return View("QuanLyUngTuyen", ketQua);
        }

        public static string RemoveDiacritics(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var normalized = input.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

    }
}
