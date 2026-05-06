// ==============================================================================
// MỤC ĐÍCH / CHỨC NĂNG FILE: Quản lý toàn bộ luồng nghiệp vụ chọn ghế và đặt vé máy bay.
// NGƯỜI VIẾT: Nguyễn Văn Huy
// THỜI GIAN SỬA ĐỔI: 06/05/2026
// PHIÊN BẢN: 1.0
// ==============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Banvemaybay.Data;
using Banvemaybay.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Banvemaybay.Controllers
{
    /// <summary>
    /// Mục đích / Chức năng: Controller điều hướng luồng Checkout (Chọn ghế, Tính tiền, Ghi nhận vé vào Database).
    /// Cấu trúc: 3 bước tương ứng 3 hàm chính ChonGhe -> NhapThongTin -> HoanTatDatVe.
    /// Người viết: Nguyễn Văn Huy - Thời gian sửa đổi: 06/05/2026
    /// </summary>
    [Authorize]
    public class VeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VeController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Mục đích: Render sơ đồ ghế và khóa các ghế đã có người mua.
        /// Ý nghĩa biến: 'danhSachGheDaMua' tổng hợp tất cả các mã ghế đã được đặt của chuyến bay hiện tại.
        /// Người viết: Nguyễn Văn Huy - Thời gian sửa: 06/05/2026
        /// </summary>
        /// <param name="chuyenBayId">ID chuyến bay đang chọn</param>
        public IActionResult ChonGhe(int chuyenBayId)
        {
            ViewBag.ChuyenBayId = chuyenBayId;
            var cacVeDaBan = _context.Ves.Where(v => v.ChuyenBayId == chuyenBayId).ToList();

            var danhSachGheDaMua = new List<string>();
            foreach (var ve in cacVeDaBan)
            {
                if (!string.IsNullOrEmpty(ve.GheDaChon))
                {
                    danhSachGheDaMua.AddRange(ve.GheDaChon.Split(','));
                }
            }

            ViewBag.GheDaMua = danhSachGheDaMua;
            return View();
        }

        [HttpPost]
        public IActionResult XacNhanGhe(int chuyenBayId, string danhSachGhe)
        {
            TempData["ChuyenBayId"] = chuyenBayId;
            TempData["GheDaChon"] = danhSachGhe;
            return RedirectToAction("NhapThongTin");
        }

        /// <summary>
        /// Mục đích: Phân loại ghế VIP/Thường và tính toán tổng tiền đơn hàng.
        /// Người viết: Nguyễn Văn Huy - Thời gian sửa: 06/05/2026
        /// </summary>
        public IActionResult NhapThongTin()
        {
            if (TempData["ChuyenBayId"] == null || TempData["GheDaChon"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int chuyenBayId = Convert.ToInt32(TempData["ChuyenBayId"]);
            string gheDaChon = TempData["GheDaChon"].ToString() ?? "";

            ViewBag.ChuyenBayId = chuyenBayId;
            ViewBag.GheDaChon = gheDaChon;
            TempData.Keep("ChuyenBayId");
            TempData.Keep("GheDaChon");

            ViewBag.TenKhachHang = User.Identity?.Name;
            ViewBag.SoDienThoaiKhach = User.FindFirst("SoDienThoai")?.Value;

            var chuyenBay = _context.ChuyenBays.Find(chuyenBayId);
            if (chuyenBay != null)
            {
                ViewBag.ChuyenBay = chuyenBay;
                ViewBag.SanBayDi = _context.SanBays.Find(chuyenBay.DiemDiId)?.TenThanhPho ?? "N/A";
                ViewBag.SanBayDen = _context.SanBays.Find(chuyenBay.DiemDenId)?.TenThanhPho ?? "N/A";

                // ----------------------------------------------------------------------
                // ĐOẠN LOGIC PHỨC TẠP: TÍNH TOÁN GIÁ VÉ THEO LOẠI GHẾ (THƯỜNG/VIP)
                // Giải thích: Bóc tách danh sách ghế (Dạng chuỗi phân cách bởi dấu ,) thành mảng.
                // Thuật toán: Nếu chỉ số ghế <= 12 thì đó là ghế VIP. Ghế VIP có giá nhân hệ số 1.5.
                // ----------------------------------------------------------------------
                var danhSachGhe = gheDaChon.Split(',');
                int soGheVip = 0;
                int soGheThuong = 0;

                foreach (var ghe in danhSachGhe)
                {
                    if (int.TryParse(ghe, out int soGheInt) && soGheInt <= 12)
                        soGheVip++;
                    else
                        soGheThuong++;
                }

                decimal giaVeGoc = chuyenBay.GiaKhuyenMai > 0 ? chuyenBay.GiaKhuyenMai : chuyenBay.GiaVe;
                decimal giaVeVip = giaVeGoc * 1.5m;

                ViewBag.SoGheThuong = soGheThuong;
                ViewBag.SoGheVip = soGheVip;
                ViewBag.GiaVeThuong = giaVeGoc;
                ViewBag.GiaVeVip = giaVeVip;
                ViewBag.TongTien = (soGheThuong * giaVeGoc) + (soGheVip * giaVeVip);
            }

            return View();
        }

        /// <summary>
        /// Mục đích: Xử lý lưu vé vào Database kèm cơ chế chống trùng lặp ghế ngồi.
        /// Người viết: Nguyễn Văn Huy - Thời gian sửa: 06/05/2026
        /// </summary>
        /// <param name="hoTen">Tên hành khách</param>
        /// <param name="soDienThoai">SĐT nhận vé</param>
        /// <param name="gheDaChon">Chuỗi danh sách ghế</param>
        /// <param name="chuyenBayId">ID chuyến bay lưu database</param>
        [HttpPost]
        public IActionResult HoanTatDatVe(string hoTen, string soDienThoai, string gheDaChon, int chuyenBayId)
        {
            if (string.IsNullOrEmpty(gheDaChon))
            {
                return Content($"<script>alert('Bạn chưa chọn ghế ngồi!'); window.location.href='/Ve/ChonGhe?chuyenBayId={chuyenBayId}';</script>", "text/html; charset=utf-8");
            }

            // ----------------------------------------------------------------------
            // ĐOẠN LOGIC PHỨC TẠP: KIỂM TRA CHỐNG GIÀNH GIẬT GHẾ KHI THANH TOÁN (RACE CONDITION)
            // Mục đích: Tránh việc 2 người cùng chọn 1 ghế ở cùng 1 thời điểm dẫn đến lỗi database.
            // Thuật toán: Dùng hàm Intersect để tìm phần giao giữa mảng ghế đang chọn và 
            // các ghế đã lưu trong DB. Nếu tồn tại phần giao (Any() == true), báo lỗi ngay.
            // ----------------------------------------------------------------------
            var cacVeDaBan = _context.Ves.Where(v => v.ChuyenBayId == chuyenBayId).ToList();
            var gheDangChon = gheDaChon.Split(',');

            foreach (var ve in cacVeDaBan)
            {
                var gheDaMua = ve.GheDaChon?.Split(',') ?? new string[0];
                if (gheDaMua.Intersect(gheDangChon).Any())
                {
                    return Content($"<script>alert('Rất tiếc, ghế bạn chọn vừa có người khác nhanh tay đặt mất. Vui lòng chọn ghế khác nhé!'); window.location.href='/Ve/ChonGhe?chuyenBayId={chuyenBayId}';</script>", "text/html; charset=utf-8");
                }
            }

            var veMoi = new Ve
            {
                ChuyenBayId = chuyenBayId,
                HoTen = hoTen ?? "",
                SoDienThoai = soDienThoai ?? "",
                GheDaChon = gheDaChon,
                NgayDat = DateTime.Now
            };

            _context.Ves.Add(veMoi);
            _context.SaveChanges();

            TempData["ThongBao"] = $"Chúc mừng {hoTen} đã đặt vé thành công! Ghế của bạn: {gheDaChon}.";
            return RedirectToAction("ThanhCong");
        }

        public IActionResult ThanhCong() => View();
    }
}