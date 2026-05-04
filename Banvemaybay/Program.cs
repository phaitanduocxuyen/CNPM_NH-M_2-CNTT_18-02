using Banvemaybay.Data;
using Banvemaybay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/TaiKhoan/DangNhap";
        options.AccessDeniedPath = "/TaiKhoan/TuChoi";

        // LOGIC TÁCH BIỆT: Nếu link bắt đầu bằng /Admin thì redirect về /Admin/Login
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    // SEED DATA ADMIN
    var admin = context.TaiKhoans.FirstOrDefault(t => t.VaiTro == "Admin");
    if (admin == null)
    {
        context.TaiKhoans.Add(new TaiKhoan
        {
            SoDienThoai = "0999999999",
            MatKhau = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9", // admin123
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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();