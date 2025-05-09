using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using WebApplication1.Services;
using static System.Net.Mime.MediaTypeNames;
using WebApplication1.SignalRHub;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity.UI.Services;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//Identity
builder.Services.AddIdentity<TaiKhoan, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;  // Email là duy nhất


    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = false;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = false;

});
//thanh toan
// Add Repository
builder.Services.AddScoped<IThanhToanRepository, ThanhToanRepository>();
// Add Services
builder.Services.AddScoped<IThanhToanService, ThanhToanService>();
// goi dich vu
// Add Repository
builder.Services.AddScoped<IGoiDichVuRepository, GoiDichVuRepository>();
// Add Services
builder.Services.AddScoped<IGoiDichVuService, GoiDichVuService>();
//add emailsender
builder.Services.AddScoped<ICustomEmailSender, EmailSender>();

//add sign
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

// Add Repository
builder.Services.AddScoped<IChatRepository, ChatRepository>();
// Add Services
builder.Services.AddScoped<IChatService, ChatService>();

// Add Repository
builder.Services.AddScoped<IDanhGiaRepository, DanhGiaRepository>();
// Add Services
builder.Services.AddScoped<IDanhGiaService, DanhGiaService>();

// Add Repository
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
// Add Services
builder.Services.AddScoped<IAccountService, AccountService>();

// Add Services
builder.Services.AddScoped<IDangTinService, DangTinService>();

// Add Repository
builder.Services.AddScoped<IDangTinRepository, DangTinRepository>();


// Add Services
builder.Services.AddScoped<IBangTinService, BangTinService>();

// Add Repository
builder.Services.AddScoped<IBangTinRepository, BangTinRepository>();


// Add Services
builder.Services.AddScoped<IUngTuyenService, UngTuyenService>();

// Add Repository
builder.Services.AddScoped<IUngTuyenRepository, UngTuyenRepository>();

// Add Services
builder.Services.AddScoped<IHoSoService, HoSoService>();

// Add Repository
builder.Services.AddScoped<IHoSoRepository, HoSoRepository>();
//add vaitro claim
builder.Services.AddScoped<IUserClaimsPrincipalFactory<TaiKhoan>, AppClaimsPrincipalFactory>();
builder.Services.AddHttpContextAccessor();
// Add Session (for TempData)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.MapStaticAssets();




app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();
app.MapHub<ChatHub>("/chatHub");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<TaiKhoan>>();

    string adminEmail = "admin@example.com";
    string adminPassword = "Admin@123";

    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        var newAdmin = new TaiKhoan
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = false,
            PhoneNumber= "0123456789",
            VaiTro = "Admin",
            TrangThai = "HoatDong",
            CCCD = "123456789999",
            FileAvata = "/uploads/images/avatar_6.png"
        }
    ;

        var result = await userManager.CreateAsync(newAdmin, adminPassword);
        if (result.Succeeded)
        {
            Console.WriteLine("✅ Admin created successfully.");
        }
        else
        {
            Console.WriteLine("❌ Failed to create admin:");
            foreach (var error in result.Errors)
                Console.WriteLine($" - {error.Description}");
        }
    }
    else
    {
        Console.WriteLine("✅ Admin already exists.");
    }
}
app.Run();
