// ==============================================================================
// MỤC ĐÍCH / CHỨC NĂNG FILE: Xử lý logic tìm kiếm và liệt kê chuyến bay ngoài Front-end.
// NGƯỜI VIẾT: Nguyễn Văn Huy
// THỜI GIAN SỬA ĐỔI: 06/05/2026
// PHIÊN BẢN: 1.0
// ==============================================================================

using Banvemaybay.Data;
using Banvemaybay.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Banvemaybay.Controllers
{
    /// <summary>
    /// Mục đích / Chức năng: Controller truy vấn dữ liệu chuyến bay phục vụ khách hàng tìm kiếm.
    /// Cấu trúc: Gồm hàm tìm kiếm theo điều kiện và hàm lấy toàn bộ danh sách hiển thị.
    /// Người viết: Nguyễn Văn Huy - Thời gian sửa đổi: 06/05/2026
    /// </summary>
    public class ChuyenBayController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChuyenBayController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Mục đích: Lọc chuyến bay dựa trên điểm xuất phát, điểm đến, ngày và số lượng người.
        /// Ý nghĩa biến: 'danhSachHopLe' lưu các chuyến bay có đủ số ghế trống yêu cầu.
        /// Người viết: Nguyễn Văn Huy - Thời gian sửa: 06/05/2026
        /// </summary>
        /// <param name="diemDi">ID Sân bay xuất phát</param>
        /// <param name="diemDen">ID Sân bay đích</param>
        /// <param name="ngayDi">Ngày khởi hành (tùy chọn)</param>
        /// <param name="loaiVe">Một chiều hoặc khứ hồi</param>
        /// <param name="soNguoiLon">Số khách người lớn</param>
        /// <param name="soTreEm">Số khách trẻ em</param>
        /// <returns>Danh sách các chuyến bay thỏa mãn vào View</returns>
        public IActionResult TimKiem(int diemDi, int diemDen, DateTime? ngayDi, string loaiVe, DateTime? ngayVe, int soNguoiLon = 1, int soTreEm = 0)
        {
            var query = _context.ChuyenBays.AsQueryable();

            query = query.Where(c => c.DiemDiId == diemDi && c.DiemDenId == diemDen);

            if (ngayDi.HasValue)
            {
                query = query.Where(c => c.ThoiGianDi.Date == ngayDi.Value.Date);
            }

            var danhSachChuyenBay = query.ToList();

            // ----------------------------------------------------------------------
            // ĐOẠN LOGIC PHỨC TẠP: KIỂM TRA SỨC CHỨA CÒN LẠI CỦA CHUYẾN BAY
            // Thuật toán: Với mỗi chuyến bay lấy ra, truy vấn tất cả các vé đã bán thuộc về nó.
            // Đếm tổng số ghế đã bán. Nếu (Tổng số ghế - Đã bán) >= Số lượng người yêu cầu 
            // thì mới đưa chuyến bay đó vào danh sách 'danhSachHopLe' để hiển thị.
            // ----------------------------------------------------------------------
            int tongGheYeuCau = soNguoiLon + soTreEm;
            var danhSachHopLe = new List<ChuyenBay>();

            foreach (var cb in danhSachChuyenBay)
            {
                var cacVeDaBan = _context.Ves.Where(v => v.ChuyenBayId == cb.Id).ToList();
                int soGheDaBan = 0;
                foreach (var ve in cacVeDaBan)
                {
                    if (!string.IsNullOrEmpty(ve.GheDaChon))
                    {
                        soGheDaBan += ve.GheDaChon.Split(',').Length;
                    }
                }

                if ((30 - soGheDaBan) >= tongGheYeuCau)
                {
                    danhSachHopLe.Add(cb);
                }
            }

            ViewBag.TenDiemDi = _context.SanBays.FirstOrDefault(s => s.Id == diemDi)?.TenThanhPho ?? "N/A";
            ViewBag.TenDiemDen = _context.SanBays.FirstOrDefault(s => s.Id == diemDen)?.TenThanhPho ?? "N/A";

            ViewBag.NgayDi = ngayDi?.ToString("dd/MM/yyyy");
            ViewBag.NgayVe = ngayVe?.ToString("dd/MM/yyyy");
            ViewBag.LoaiVe = loaiVe == "khuhoi" ? "Khứ hồi" : "Một chiều";
            ViewBag.SoNguoiLon = soNguoiLon;
            ViewBag.SoTreEm = soTreEm;

            return View(danhSachHopLe);
        }

        /// <summary>
        /// Mục đích: Lấy toàn bộ các chuyến bay sắp khởi hành (Dành cho mục Gợi ý).
        /// Người viết: Nguyễn Văn Huy - Thời gian sửa: 06/05/2026
        /// </summary>
        /// <returns>Danh sách chuyến bay có thời gian >= hiện tại</returns>
        public IActionResult DanhSach()
        {
            ViewBag.DanhSachSanBay = _context.SanBays.ToList();

            var danhSachChuyenBay = _context.ChuyenBays
                .Where(c => c.ThoiGianDi >= DateTime.Now)
                .OrderBy(c => c.ThoiGianDi)
                .ToList();

            return View(danhSachChuyenBay);
        }
    }
}