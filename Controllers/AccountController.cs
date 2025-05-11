
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services;
using WebApplication1.Repositories;
using WebApplication1.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Hosting;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly IAccountService _accountService;
        private readonly IAccountRepository _accountRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IDanhGiaService _danhGiaService;
        public AccountController(UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager, IAccountService accountService,IAccountRepository accountRepository, ApplicationDbContext applicationDbContext, ILogger<AccountController> logger, IWebHostEnvironment hostEnvironment, IDanhGiaService danhGiaService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountService = accountService;
            _accountRepository = accountRepository;
            _context = applicationDbContext;
            _logger = logger;
            _hostingEnvironment = hostEnvironment;
            _danhGiaService = danhGiaService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
           
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Action("Index", "Home");
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                bool result = await _accountService.login(model);
                if (result)
                {
                    // Sau khi đăng nhập thành công, tải lại user từ DB để đảm bảo claims mới nhất
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    // Refresh user từ DB (rất quan trọng nếu bạn có thay đổi avatar ở đâu đó)
                    await _userManager.UpdateAsync(user); // nếu cần cập nhật gì nữa

                    await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    //var user = await _userManager.FindByEmailAsync(model.Email);
                     var vaiTro = user.VaiTro;
                    // ✅ Thêm vào đây để refresh Claims
                    //await HttpContext.SignOutAsync();
                    // await _signInManager.SignInAsync(user, isPersistent: false);
                    //await _signInManager.RefreshSignInAsync(user);
                    // Sign-out cũ
                    await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

                    // Đăng nhập lại với claims mới
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    if (vaiTro == "Admin")
                    {
                       
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (vaiTro == "PhuHuynh")
                    {
                        
                        return RedirectToAction("Index", "Home");
                    }
                    else if (vaiTro == "GiaSu")
                    {
                       
                        
                        return RedirectToAction("Index", "GiaSu");
                    }

                    return RedirectToAction("Index", "Home"); // fallback
                }
            }
            return View(model);
        }
       
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterStep1(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewData["ReturnUrl"] = returnUrl;
            // Khôi phục dữ liệu từ TempData nếu có (khi người dùng quay lại)
            var model = new RegisterStep1ViewModel
            {
                VaiTro = TempData["VaiTro"]?.ToString(),
                Email = TempData["Email"]?.ToString(),
                MatKhau = TempData["MatKhau"]?.ToString()
            };
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterStep1(RegisterStep1ViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Action("RegisterStep2", "Account");
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Lưu thông tin vào TempData để sử dụng trong bước tiếp theo
                TempData["VaiTro"] = model.VaiTro;
                TempData["Email"] = model.Email;
                TempData["MatKhau"] = model.MatKhau;
                return RedirectToAction("RegisterStep2");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterStep2(string returnUrl = null)
        {
            returnUrl ??= Url.Action("Main", "Home");
            ViewData["ReturnUrl"] = returnUrl;
            if (TempData["Email"] == null || TempData["MatKhau"] == null || TempData["VaiTro"] == null)
            {
                return RedirectToAction("RegisterStep1"); // Quay lại Step1 nếu thiếu dữ liệu
            }

            var model = new RegisterStep2ViewModel
            {
                Email = TempData["Email"]?.ToString() ?? "",
                MatKhau = TempData["MatKhau"]?.ToString() ?? "",
                VaiTro = TempData["VaiTro"]?.ToString() ?? ""
            };

            // Giữ lại TempData cho request tiếp theo
            KeepStep1Data();

            return View(model);
           /* // Khôi phục dữ liệu từ TempData nếu có (khi người dùng quay lại)
            var model = new RegisterStep2ViewModel
            {
                VaiTro = TempData["VaiTro"]?.ToString(),
                Email = TempData["Email"]?.ToString(),
                MatKhau = TempData["MatKhau"]?.ToString()
            };
            return View(model);*/
        }
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStep2(RegisterStep2ViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Action("RegisterSuccess", "Account");
            ViewData["ReturnUrl"] = returnUrl;
            // Giữ lại TempData cho request tiếp theo
            KeepStep1Data();
            //if (!HasValidStep1Data())
            // {
            //   _logger.LogWarning("Mất dữ liệu bước 1, quay lại RegisterStep1.");
            //   return RedirectToAction(nameof(RegisterStep1));
            //}

            model.VaiTro = TempData["VaiTro"]?.ToString();
            model.Email = TempData["Email"]?.ToString();
            model.MatKhau = TempData["MatKhau"]?.ToString();

            try
            {
                

                    // Validate files
                    if (model.sFile_CCCD == null || model.sFile_CCCD.Length == 0)
                    {
                        ModelState.AddModelError("sFile_CCCD", "Vui lòng tải lên ảnh CCCD");
                        KeepStep1Data();
                        return View(model);
                    }

                    // Validate file types and sizes
                    var allowedImageTypes = new[] { "image/jpeg", "image/png" };
                    var maxFileSize = 5 * 1024 * 1024; // 5MB

                    if (!allowedImageTypes.Contains(model.sFile_CCCD.ContentType))
                    {
                        ModelState.AddModelError("sFile_CCCD", "Chỉ chấp nhận file ảnh JPEG hoặc PNG");
                        KeepStep1Data();
                        return View(model);
                    }

                    if (model.sFile_CCCD.Length > maxFileSize)
                    {
                        ModelState.AddModelError("sFile_CCCD", "File ảnh không được vượt quá 5MB");
                        KeepStep1Data();
                        return View(model);
                    }

                    if (model.sFile_Avata != null)
                    {
                        if (!allowedImageTypes.Contains(model.sFile_Avata.ContentType))
                        {
                            ModelState.AddModelError("sFile_Avata", "Chỉ chấp nhận file ảnh JPEG hoặc PNG");
                            KeepStep1Data();
                            return View(model);
                        }

                        if (model.sFile_Avata.Length > maxFileSize)
                        {
                            ModelState.AddModelError("sFile_Avata", "File ảnh không được vượt quá 5MB");
                            KeepStep1Data();
                            return View(model);
                        }
                        // return View(model);
                    }
                    bool emailExists = await _accountService.EmailExistsAsync(model.Email);
                    if (emailExists)
                    {
                        ModelState.AddModelError(string.Empty, "Email đã tồn tại.");
                        return View(model);
                    }
                    bool phoneExists = await _accountService.PhoneExistsAsync(model.SDT);
                    if (phoneExists)
                    {
                        ModelState.AddModelError(string.Empty, "Số điện thoại đã tồn tại.");
                        return View(model);
                    }
                    bool cccdExists = await _accountService.CCCDExistAsync(model.CCCD);
                    if (cccdExists)
                    {
                        ModelState.AddModelError(string.Empty, "CCCD đã tồn tại.");
                        return View(model);
                    }
                    // Validate file
                    // Xử lý upload file và đăng ký
                    string cccdPath = await UploadFile(model.sFile_CCCD, "cccd");
                    string avatarPath = model.sFile_Avata != null
                        ? await UploadFile(model.sFile_Avata, "avatars")
                        : null;
                    //  Gán vào model để truyền vào RegisterAsync
                    model.sFile_CCCD_Path = cccdPath;
                    model.sFile_Avata_Path = avatarPath;
                    _logger.LogError($" + {model.sFile_CCCD_Path.ToString()} + {model.sFile_Avata_Path.ToString()}");
                    var result = await _accountService.CreateTaiKhoanAsync(model);
                    // Thêm từng lỗi trong IdentityResult vào ModelState
                    if (!result)
                    {
                        ModelState.AddModelError("", "Đăng ký không thành công. Vui lòng thử lại sau.");
                        KeepStep1Data();
                        return View(model);
                    }
                    // Đăng nhập người dùng sau khi đăng ký thành công
                    /* var user = await _userManager.FindByEmailAsync(model.Email);
                     if (user != null)
                     {
                         await _signInManager.SignInAsync(user, isPersistent: false);
                         _logger.LogInformation($"User {user.UserName} logged in.");
                         return LocalRedirect(returnUrl);
                     }*/
                    TempData.Remove("VaiTro");
                    TempData.Remove("Email");
                    TempData.Remove("MatKhau");

                    return RedirectToAction(nameof(Login));

                
            }
            // If we got this far, something failed, redisplay form
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi đăng ký tài khoản");
                ModelState.AddModelError("", "Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại.");
                KeepStep1Data();
                return View(model);

            }
            }


        [HttpGet]
        public async Task<IActionResult> CapNhatTaiKhoan()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new RegisterStep2ViewModel
            {
                id = user.Id,
                HoTen = user.UserName,
                Email = user.Email,
                SDT = user.PhoneNumber,
                CCCD = user.CCCD,
                DiaChi = user.DiaChi,
                sFile_Avata_Path = user.FileAvata,
                sFile_CCCD_Path = user.FileCCCD
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CapNhatTaiKhoan(RegisterStep2ViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Check trùng CCCD và SDT nếu có thay đổi
            // Check trùng CCCD và SDT nếu có thay đổi
            if (await _accountService.PhoneExistsAsync(model.SDT, excludeUserId: user.Id))
                ModelState.AddModelError("SDT", "SĐT đã tồn tại.");

            if (await _accountService.CCCDExistAsync(model.CCCD, excludeUserId: user.Id))
                ModelState.AddModelError("CCCD", "CCCD đã tồn tại.");

            // if (!ModelState.IsValid) return View(model);

            // Upload ảnh nếu có
            if (model.sFile_Avata != null)
            {
                user.FileAvata = await UploadFile(model.sFile_Avata, "avatars");
            }
            else
            {
                user.FileAvata = model.sFile_Avata_Path; // giữ lại ảnh cũ nếu không thay
            }

            if (model.sFile_CCCD != null)
            {
                user.FileCCCD = await UploadFile(model.sFile_CCCD, "cccd");
            }
            else
            {
                user.FileCCCD = model.sFile_CCCD_Path;
            }

            // Cập nhật thông tin khác
            user.UserName = model.HoTen;
            user.PhoneNumber = model.SDT;
            user.CCCD = model.CCCD;
            user.DiaChi = model.DiaChi;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Cập nhật thành công.";
                return RedirectToAction("Profile");
            }
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); // Xoá cookie xác thực
            return RedirectToAction("Login", "Account"); // Chuyển về trang chủ hoặc trang đăng nhập
        }
        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            ViewBag.SuccessMessage = "Mật khẩu đã được thay đổi thành công!";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DoiMatKhau(ChangePasswordViewModel model)
        {
           

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                ViewBag.SuccessMessage = "Mật khẩu đã được thay đổi thành công!";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");
           
            // Lấy danh sách đánh giá
            var danhGias = await _danhGiaService.GetDanhGiaByGiaSuIdAsync(user.Id);
            var diemTB = await _danhGiaService.GetAverageRatingAsync(user.Id);

            if (user.VaiTro == "giasu" )
            {
                var hoso = _context.HoSos.FirstOrDefault(hoso => hoso.FK_iMaTK == user.Id); 
                if(hoso == null)
                {
                    var model = new ProfileViewModel
                    {
                        VaiTro = user.VaiTro,
                        Email = user.Email,
                        MatKhau = user.PasswordHash,
                        CCCD = user.CCCD,
                        SDT = user.PhoneNumber,
                        HoTen = user.UserName,
                        DiaChi = user.DiaChi,
                        sFile_Avata_Path = user.FileAvata,
                        sFile_CCCD_Path = user.FileCCCD,
                        sFile_Avata = null,
                        sFile_CCCD = null,
                        // hien thị goi cuoc cua nguoi dung
                       //THEM
                        //THEM

                        // ➕ Truyền dữ liệu đánh giá vào ViewModel
                        DanhGias = danhGias,
                        DiemTrungBinh = diemTB
                    };
                    return View(model);
                }
                else {
                    // them
                    var model = new ProfileViewModel
                    {
                        VaiTro = user.VaiTro,
                        Email = user.Email,
                        MatKhau = user.PasswordHash,
                        CCCD = user.CCCD,
                        SDT = user.PhoneNumber,
                        HoTen = user.UserName,
                        DiaChi = user.DiaChi,
                        sFile_Avata_Path = user.FileAvata,
                        sFile_CCCD_Path = user.FileCCCD,
                        sFile_Avata = null,
                        sFile_CCCD = null,
                        // hien thị goi cuoc cua nguoi dung
                        GoiCuoc = user.GoiCuoc,//THEM
                        TieuDeCV = hoso.sTieuDe,
                        // ➕ Truyền dữ liệu đánh giá vào ViewModel
                        DanhGias = danhGias,
                        DiemTrungBinh = diemTB
                    };
                    return View(model);
                }
               
            }
            if(user.VaiTro=="phuhuynh")
            {
                var model = new ProfileViewModel
                {
                    VaiTro = user.VaiTro,
                    Email = user.Email,
                    MatKhau = user.PasswordHash,
                    CCCD = user.CCCD,
                    SDT = user.PhoneNumber,
                    HoTen = user.UserName,
                    DiaChi = user.DiaChi,
                    sFile_Avata_Path = user.FileAvata,
                    sFile_CCCD_Path = user.FileCCCD,
                    sFile_Avata = null,
                    sFile_CCCD = null,
                    // hien thị goi cuoc cua nguoi dung
                    GoiCuoc = user.GoiCuoc,
                    //tengoi=user.ThanhToans.FirstOrDefault()?.GoiDichVu.TenGoi, //THEM
                    // ➕ Truyền dữ liệu đánh giá vào ViewModel
                    DanhGias = danhGias,
                    DiemTrungBinh = diemTB
                };
                return View(model);
            }

                return View();
        }
        private void KeepStep1Data()
        {
            TempData.Keep("Email");
            TempData.Keep("MatKhau");
            TempData.Keep("VaiTro");
        }
        private async Task<string> UploadFile(IFormFile file, string folderName)
        {
            //kiem tra xem file có tồn tại hay không 
            if (file == null || file.Length == 0)
                return null;
            //tạo đường dẫn thư mục 
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", folderName);
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
        private bool HasValidStep1Data()
        {
            return !string.IsNullOrEmpty(TempData["VaiTro"] as string) &&
                   !string.IsNullOrEmpty(TempData["Email"] as string) &&
                   !string.IsNullOrEmpty(TempData["MatKhau"] as string);
        }
    }
}
