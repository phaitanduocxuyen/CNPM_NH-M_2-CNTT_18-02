/**
 * 1. File: SanBay.cs
 * Mục đích / Chức năng: Định nghĩa cấu trúc dữ liệu cho đối tượng Sân bay, phục vụ việc quản lý điểm đi và điểm đến.
 * Người viết: pHạm Anh Tú
 * Thời gian sửa đổi: 06/05/2026
 * Phiên bản: 1.0
 */

namespace Banvemaybay.Models
{
    /**
     * 3. Class: SanBay
     * Mục đích / Chức năng: Lớp mô tả thực thể Sân bay trong hệ thống, chứa các thông tin định danh và vị trí địa lý.
     * Cấu trúc: Bao gồm mã sân bay và tên thành phố tương ứng.
     * Người viết / tg sửa đổi: pHạm Anh Tú - Cập nhật 06/05/2026
     */
    public class SanBay
    {
        /** 
         * Ý nghĩa các biến (Properties):
         * Id: Mã định danh duy nhất (Primary Key) của sân bay trong cơ sở dữ liệu.
         * MaSanBay: Mã viết tắt chuẩn quốc tế của sân bay (ví dụ: HAN cho Nội Bài, SGN cho Tân Sơn Nhất).
         * TenThanhPho: Tên thành phố nơi sân bay tọa lạc để hiển thị cho người dùng dễ nhận biết.
         */

        public int Id { get; set; }
        public string MaSanBay { get; set; } = string.Empty;
        public string TenThanhPho { get; set; } = string.Empty;
    }
}