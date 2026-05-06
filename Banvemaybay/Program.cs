/**
 * 1. File: Program.cs
 * Mục đích / Chức năng: Cấu hình các dịch vụ (Services), thiết lập Middleware và khởi chạy ứng dụng web.
 * Người viết: pHạm Anh Tú
 * Thời gian sửa đổi: 06/05/2026
 * Phiên bản: 1.0
 */

using Banvemaybay.Data;
using Banvemaybay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// --- 2. Cấu hình Dịch vụ (Services) ---

builder.Services.AddControllersWithViews();

// Cấu hình Database: Kết nối tới SQL Server dựa trên chuỗi kết nối trong appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/**
 * 5. Đoạn logic phức tạp: Cấu hình Xác thực (Authentication)
 * Giải thích: Sử dụng Cookie để duy trì đăng nhập. 
 * Đặc biệt: Tách biệt luồng chuyển hướng khi chưa đăng nhập giữa người dùng thông thường và quản trị viên (Admin).
 */
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/TaiKhoan/DangNhap";
        options.AccessDeniedPath = "/TaiKhoan/TuChoi";

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                // Nếu người dùng truy cập vào khu vực /Admin mà chưa đăng nhập, sẽ bị đưa tới trang Login riêng của Admin
                if (context.Request.Path.StartsWithSegments("/Admin"))
                {
                    context.Response.Redirect("/Admin/Login");
                }
                else
                {
                    context.Response.Redirect(context.RedirectUri);
                }
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// --- 5. Đoạn logic phức tạp: Khởi tạo dữ liệu (Seeding Data) ---
/**
 * Mục đích: Tự động chạy Migration và tạo tài khoản Admin mặc định nếu chưa có trong DB.
 * Thuật toán: Kiểm tra sự tồn tại của VaiTro == "Admin" trước khi thêm mới hoặc cập nhật mật khẩu.
 */
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    var admin = context.TaiKhoans.FirstOrDefault(t => t.VaiTro == "Admin");
    if (admin == null)
    {
        context.TaiKhoans.Add(new TaiKhoan
        {
            SoDienThoai = "0999999999",
            MatKhau = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9", // mật khẩu băm của: admin123
            HoTen = "Quản Trị Viên",
            VaiTro = "Admin"
        });
    }
    else
    {
        admin.MatKhau = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9";
        context.TaiKhoans.Update(admin);
    }
    context.SaveChanges();
}

// --- Cấu hình Middleware (Pipeline) ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Thứ tự quan trọng: Phải xác thực (Authentication) trước khi phân quyền (Authorization)
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// 4. Function/Route: Định nghĩa luồng điều hướng mặc định của ứng dụng
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();