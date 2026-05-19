/**
 * 1. File: Ve.cs
 * Mục đích / Chức năng: Quản lý thông tin chi tiết về việc đặt vé của khách hàng cho một chuyến bay cụ thể.
 * Người viết: pHạm Anh Tú
 * Thời gian sửa đổi: 06/05/2026
 * Phiên bản: 1.0
 */

using System;

namespace Banvemaybay.Models
{
    /**
     * 3. Class: Ve
     * Mục đích / Chức năng: Đại diện cho thực thể Vé máy bay, lưu trữ thông tin hành khách và vị trí ghế ngồi.
     * Cấu trúc: Liên kết với ChuyenBayId và lưu thông tin định danh khách hàng.
     * Người viết / tg sửa đổi: pHạm Anh Tú - Cập nhật 06/05/2026
     */
    public class Ve
    {
        /** 
         * Ý nghĩa các biến (Properties):
         * Id: Khóa chính của bản ghi vé trong cơ sở dữ liệu.
         * ChuyenBayId: Khóa ngoại liên kết tới bảng ChuyenBay để biết vé này thuộc chuyến nào.
         * HoTen: Họ tên của người đi (hành khách) in trên vé.
         * SoDienThoai: Thông tin liên lạc của khách hàng để gửi mã vé hoặc thông báo.
         * GheDaChon: Mã số ghế mà khách đã chọn trên máy bay (ví dụ: 12A, 14B).
         * NgayDat: Thời điểm khách hàng thực hiện giao dịch đặt vé thành công.
         */

        public int Id { get; set; }
        public int ChuyenBayId { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string GheDaChon { get; set; } = string.Empty;
        public DateTime NgayDat { get; set; } = DateTime.Now;
    }
}