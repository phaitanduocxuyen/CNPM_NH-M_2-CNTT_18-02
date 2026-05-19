// ==============================================================================
// MỤC ĐÍCH / CHỨC NĂNG FILE: Chứa các chức năng dành riêng cho Quản trị viên (Admin)
// NGƯỜI VIẾT: Nguyễn Văn Huy
// THỜI GIAN SỬA ĐỔI: 06/05/2026
// PHIÊN BẢN: 1.1 (Đã tích hợp tự động tạo chuyến bay khứ hồi)
// ==============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Banvemaybay.Data;
using Banvemaybay.Models;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System;

namespace Banvemaybay.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Mục đích: Băm mật khẩu đầu vào bằng thuật toán SHA256 để bảo mật.
        /// </summary>
        private string MaHoaMatKhau(string matKhau)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(matKhau));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }

        [AllowAnonymous]
        [HttpGet("")]
        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (User.IsInRole("Admin")) return RedirectToAction("Index");
            return View();
        }

        /// <summary>
        /// Xử lý logic đăng nhập hệ thống Admin
        /// </summary>
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(string matKhauAdmin)
        {
            if (string.IsNullOrEmpty(matKhauAdmin))
            {
                ViewBag.Loi = "Vui lòng nhập mã truy cập!";
                return View();
            }

            string matKhauMaHoa = MaHoaMatKhau(matKhauAdmin);
            var adminUser = _context.TaiKhoans.FirstOrDefault(t => t.VaiTro == "Admin" && t.MatKhau == matKhauMaHoa);

            if (adminUser != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, adminUser.HoTen),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index");
            }

            ViewBag.Loi = "Mã bảo mật không chính xác!";
            return View();
        }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            ViewBag.TongSoChuyenBay = _context.ChuyenBays.Count();
            ViewBag.TongSoSanBay = _context.SanBays.Count();
            ViewBag.TongSoVeDaBan = _context.Ves.Count();
            ViewBag.TongKhachHang = _context.TaiKhoans.Count(t => t.VaiTro == "KhachHang");

            var danhSachVeMoi = _context.Ves.OrderByDescending(v => v.NgayDat).Take(10).ToList();

            int namHienTai = DateTime.Now.Year;
            var veNamNay = _context.Ves.Where(v => v.NgayDat.Year == namHienTai).ToList();
            var danhSachChuyenBay = _context.ChuyenBays.ToList();

            decimal[] doanhThuThang = new decimal[12];

            foreach (var ve in veNamNay)
            {
                var cb = danhSachChuyenBay.FirstOrDefault(c => c.Id == ve.ChuyenBayId);
                if (cb != null && !string.IsNullOrEmpty(ve.GheDaChon))
                {
                    int soGhe = ve.GheDaChon.Split(',').Length;
                    decimal giaApDung = cb.GiaKhuyenMai > 0 ? cb.GiaKhuyenMai : cb.GiaVe;
                    doanhThuThang[ve.NgayDat.Month - 1] += (soGhe * giaApDung);
                }
            }

            ViewBag.DoanhThuThang = string.Join(",", doanhThuThang);
            ViewBag.NamHienTai = namHienTai;

            return View(danhSachVeMoi);
        }

        [HttpGet("QuanLyVe")]
        public IActionResult QuanLyVe()
        {
            var danhSachVe = _context.Ves.OrderByDescending(v => v.NgayDat).ToList();
            ViewBag.DanhSachChuyenBay = _context.ChuyenBays.ToList();
            return View(danhSachVe);
        }

        [HttpGet("XoaVe")]
        public IActionResult XoaVe(int id)
        {
            var ve = _context.Ves.Find(id);
            if (ve != null) { _context.Ves.Remove(ve); _context.SaveChanges(); TempData["ThongBao"] = "Xóa vé thành công!"; }
            return RedirectToAction("QuanLyVe");
        }

        [HttpGet("QuanLyChuyenBay")]
        public IActionResult QuanLyChuyenBay()
        {
            var ds = _context.ChuyenBays.OrderByDescending(c => c.ThoiGianDi).ToList();
            ViewBag.DanhSachSanBay = _context.SanBays.ToList();
            return View(ds);
        }

        [HttpGet("ThemChuyenBay")]
        public IActionResult ThemChuyenBay()
        {
            ViewBag.DanhSachSanBay = _context.SanBays.ToList();
            return View();
        }

        /// <summary>
        /// Thêm chuyến bay mới (Có lựa chọn tự động đảo ngược tạo chuyến về khứ hồi)
        /// </summary>
        [HttpPost("ThemChuyenBay")]
        public IActionResult ThemChuyenBay(ChuyenBay chuyenBay, bool taoChuyenVe, string maChuyenVe, DateTime? thoiGianDiVe, DateTime? thoiGianDenVe)
        {
            if (ModelState.IsValid)
            {
                _context.ChuyenBays.Add(chuyenBay);

                if (taoChuyenVe && !string.IsNullOrEmpty(maChuyenVe) && thoiGianDiVe.HasValue && thoiGianDenVe.HasValue)
                {
                    var chuyenBayVe = new ChuyenBay
                    {
                        MaChuyenBay = maChuyenVe.ToUpper(),
                        HangHangKhong = chuyenBay.HangHangKhong,
                        GiaVe = chuyenBay.GiaVe,
                        GiaKhuyenMai = chuyenBay.GiaKhuyenMai,
                        DiemDiId = chuyenBay.DiemDenId,
                        DiemDenId = chuyenBay.DiemDiId,
                        ThoiGianDi = thoiGianDiVe.Value,
                        ThoiGianDen = thoiGianDenVe.Value
                    };

                    _context.ChuyenBays.Add(chuyenBayVe);
                }

                _context.SaveChanges();
                TempData["ThongBao"] = "Thêm các chuyến bay mới thành công!";
                return RedirectToAction("QuanLyChuyenBay");
            }

            ViewBag.DanhSachSanBay = _context.SanBays.ToList();
            return View(chuyenBay);
        }

        [HttpGet("SuaChuyenBay/{id}")]
        public IActionResult SuaChuyenBay(int id)
        {
            var cb = _context.ChuyenBays.Find(id);
            ViewBag.DanhSachSanBay = _context.SanBays.ToList();
            return View(cb);
        }

        [HttpPost("SuaChuyenBay/{id}")]
        public IActionResult SuaChuyenBay(ChuyenBay model)
        {
            if (ModelState.IsValid) { _context.ChuyenBays.Update(model); _context.SaveChanges(); return RedirectToAction("QuanLyChuyenBay"); }
            return View(model);
        }

        [HttpGet("XoaChuyenBay/{id}")]
        public IActionResult XoaChuyenBay(int id)
        {
            if (!_context.Ves.Any(v => v.ChuyenBayId == id))
            {
                var cb = _context.ChuyenBays.Find(id);
                if (cb != null) { _context.ChuyenBays.Remove(cb); _context.SaveChanges(); }
            }
            return RedirectToAction("QuanLyChuyenBay");
        }

        [HttpGet("QuanLySanBay")]
        public IActionResult QuanLySanBay()
        {
            var dsSanBay = _context.SanBays.OrderBy(s => s.TenThanhPho).ToList();
            return View(dsSanBay);
        }

        [HttpPost("ThemSanBay")]
        public IActionResult ThemSanBay(SanBay model)
        {
            if (ModelState.IsValid)
            {
                if (_context.SanBays.Any(s => s.MaSanBay == model.MaSanBay)) { TempData["Loi"] = "Mã sân bay đã tồn tại!"; return RedirectToAction("QuanLySanBay"); }
                _context.SanBays.Add(model); _context.SaveChanges(); TempData["ThongBao"] = "Thêm sân bay mới thành công!";
            }
            return RedirectToAction("QuanLySanBay");
        }

        [HttpGet("XoaSanBay/{id}")]
        public IActionResult XoaSanBay(int id)
        {
            if (_context.ChuyenBays.Any(c => c.DiemDiId == id || c.DiemDenId == id)) { TempData["Loi"] = "Không thể xóa! Đang có chuyến bay sử dụng sân bay này."; return RedirectToAction("QuanLySanBay"); }
            var sb = _context.SanBays.Find(id);
            if (sb != null) { _context.SanBays.Remove(sb); _context.SaveChanges(); TempData["ThongBao"] = "Đã xóa sân bay thành công!"; }
            return RedirectToAction("QuanLySanBay");
        }

        [HttpGet("QuanLyKhachHang")]
        public IActionResult QuanLyKhachHang()
        {
            var dsKhachHang = _context.TaiKhoans.Where(t => t.VaiTro == "KhachHang").OrderByDescending(t => t.Id).ToList();
            var thongKeVe = new Dictionary<string, int>();
            foreach (var kh in dsKhachHang)
            {
                thongKeVe[kh.SoDienThoai] = _context.Ves.Count(v => v.SoDienThoai == kh.SoDienThoai);
            }
            ViewBag.ThongKeVe = thongKeVe;

            return View(dsKhachHang);
        }

        [HttpGet("XoaKhachHang/{id}")]
        public IActionResult XoaKhachHang(int id)
        {
            var kh = _context.TaiKhoans.FirstOrDefault(t => t.Id == id && t.VaiTro == "KhachHang");
            if (kh != null)
            {
                _context.TaiKhoans.Remove(kh);
                _context.SaveChanges();
                TempData["ThongBao"] = $"Đã xóa tài khoản của khách hàng {kh.HoTen} thành công!";
            }
            return RedirectToAction("QuanLyKhachHang");
        }
    }
}