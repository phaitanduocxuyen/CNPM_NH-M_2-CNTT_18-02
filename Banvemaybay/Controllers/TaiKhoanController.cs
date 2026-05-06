// ==============================================================================
// MỤC ĐÍCH / CHỨC NĂNG FILE: Xử lý nghiệp vụ liên quan đến tài khoản người dùng (Đăng ký, Đăng nhập, Hồ sơ).
// NGƯỜI VIẾT: Nguyễn Văn Huy
// THỜI GIAN SỬA ĐỔI: 06/05/2026
// PHIÊN BẢN: 1.0
// ==============================================================================

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
    /// <summary>
    /// Mục đích / Chức năng: Controller quản lý định danh (Identity) cho đối tượng Khách hàng.
    /// Cấu trúc: Các hàm hỗ trợ Đăng ký, Xác thực, Ủy quyền và Quản lý hồ sơ cá nhân.
    /// Người viết: Nguyễn Văn Huy - Thời gian sửa đổi: 06/05/2026
    /// </summary>
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Mục đích: Băm mật khẩu (Dùng chung logic mã hóa từ AdminController).
        /// Người viết: Nguyễn Văn Huy - Thời gian sửa: 06/05/2026
        /// </summary>
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

        public IActionResult DangKy() => View();

        /// <summary>
        /// Mục đích: Ghi nhận thông tin người dùng mới vào database, gán mặc định vai trò Khách hàng.
        /// Người viết: Nguyễn Văn Huy - Thời gian sửa: 06/05/2026
        /// </summary>
        /// <param name="model">Đối tượng Tài Khoản chứa dữ liệu form</param>
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
                model.MatKhau = MaHoaMatKhau(model.MatKhau);
                _context.TaiKhoans.Add(model);
                _context.SaveChanges();

                TempData["ThongBao"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("DangNhap");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DangNhap(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Mục đích: Xác thực tài khoản dựa trên SĐT và mật khẩu (Chỉ cấp quyền cho role Khách Hàng).
        /// Ý nghĩa biến: 'returnUrl' để điều hướng lại trang cũ sau khi đăng nhập xong.
        /// Người viết: Nguyễn Văn Huy - Thời gian sửa: 06/05/2026
        /// </summary>
        /// <param name="soDienThoai">SĐT người dùng nhập</param>
        /// <param name="matKhau">Mật khẩu người dùng nhập</param>
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

            ViewBag.Loi = "Số điện thoại hoặc mật khẩu không chính xác!";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult TuChoi()
        {
            return Content("Bạn không có quyền truy cập trang này!");
        }

        /// <summary>
        /// Mục đích: Truy xuất thông tin cá nhân và lịch sử vé dựa trên Claim SĐT.
        /// Người viết: Nguyễn Văn Huy - Thời gian sửa: 06/05/2026
        /// </summary>
        [Authorize]
        [HttpGet]
        public IActionResult HoSo()
        {
            var sdt = User.FindFirst("SoDienThoai")?.Value;

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