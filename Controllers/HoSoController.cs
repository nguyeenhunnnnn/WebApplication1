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

        public HoSoController(ILogger<HoSoController> logger,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context,
            IConfiguration configuration,
            UserManager<TaiKhoan> userManager,
            SignInManager<TaiKhoan> signInManager,
            IHoSoService hoSoService
            )
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _hoSoService = hoSoService;
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
                TempData["Success"] = "Đăng tin thành công!";
                return RedirectToAction("Index");// chuyển hướng trang danh sách 
            }
            else
            {
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
                TempData["Success"] = "Xóa tin thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Xóa tin không thành công!";
                return RedirectToAction("Index");
            }
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
            if (!string.IsNullOrEmpty(tieude))
            {
                hoSoList = hoSoList.Where(bd => bd.sTieuDe.Contains(tieude)).ToList();
            }
            if (!string.IsNullOrEmpty(hocvan))
            {
               
                    hoSoList = hoSoList.Where(bd => bd.sBangCap.Contains(hocvan)).ToList();
                
            }
            if (!string.IsNullOrEmpty(kinhnghiem))
            {
                hoSoList = hoSoList.Where(bd => bd.sKinhNghiem.Contains(kinhnghiem)).ToList();
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

    }
 }
