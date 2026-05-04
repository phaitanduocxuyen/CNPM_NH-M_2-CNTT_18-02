using Microsoft.AspNetCore.Mvc;
using Banvemaybay.Data;
using Banvemaybay.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;

namespace Banvemaybay.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hàm hỗ trợ mã hóa mật khẩu SHA256
        private string MaHoaMatKhau(string matKhau)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(matKhau));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // --- ĐĂNG KÝ ---
        public IActionResult DangKy() => View();

        [HttpPost]
        public IActionResult DangKy(TaiKhoan model)
        {
            if (ModelState.IsValid)
            {
                if (_context.TaiKhoans.Any(t => t.SoDienThoai == model.SoDienThoai))
                {
                    ViewBag.Loi = "Số điện thoại này đã được đăng ký!";
                    return View(model);
                }

                model.VaiTro = "KhachHang";
                model.MatKhau = MaHoaMatKhau(model.MatKhau); // Mã hóa mật khẩu trước khi lưu
                _context.TaiKhoans.Add(model);
                _context.SaveChanges();

                TempData["ThongBao"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("DangNhap");
            }
            return View(model);
        }

        // --- ĐĂNG NHẬP (DÀNH RIÊNG CHO KHÁCH HÀNG) ---
        [HttpGet]
        public IActionResult DangNhap(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(string soDienThoai, string matKhau, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(soDienThoai) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.Loi = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            string matKhauMaHoa = MaHoaMatKhau(matKhau);

            // TÁCH BIỆT HOÀN TOÀN: Chỉ tìm tài khoản có vai trò KhachHang
            var user = _context.TaiKhoans.FirstOrDefault(t =>
                t.SoDienThoai == soDienThoai &&
                t.MatKhau == matKhauMaHoa &&
                t.VaiTro == "KhachHang");

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.HoTen),
                    new Claim(ClaimTypes.Role, user.VaiTro),
                    new Claim("SoDienThoai", user.SoDienThoai)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            // Nếu thông tin khớp với Admin hoặc sai mật khẩu đều báo lỗi chung để bảo mật
            ViewBag.Loi = "Số điện thoại hoặc mật khẩu không chính xác!";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // --- ĐĂNG XUẤT ---
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // --- TỪ CHỐI TRUY CẬP ---
        public IActionResult TuChoi()
        {
            return Content("Bạn không có quyền truy cập trang này!");
        }

        // --- HỒ SƠ CÁ NHÂN ---
        [Authorize]
        [HttpGet]
        public IActionResult HoSo()
        {
            var sdt = User.FindFirst("SoDienThoai")?.Value;

            // Nếu Admin cố tình vào trang hồ sơ khách hàng mà không có SĐT lưu trong Claim
            if (User.IsInRole("Admin") && string.IsNullOrEmpty(sdt))
            {
                return RedirectToAction("Index", "Admin");
            }

            var user = _context.TaiKhoans.FirstOrDefault(t => t.SoDienThoai == sdt);
            if (user == null) return RedirectToAction("DangXuat");

            ViewBag.VeCuaToi = _context.Ves.Where(v => v.SoDienThoai == sdt).OrderByDescending(v => v.NgayDat).ToList();
            ViewBag.DanhSachChuyenBay = _context.ChuyenBays.ToList();
            ViewBag.DanhSachSanBay = _context.SanBays.ToList();

            return View(user);
        }
    }
}