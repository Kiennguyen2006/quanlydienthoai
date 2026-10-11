using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlydienthoai.Models;

public partial class HoaDon
{
    [Key]
    [StringLength(10)]
    public string MaHoaDon { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
    [StringLength(10)]
    public string MaNhanVien { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
    [StringLength(10)]
    public string MaKhachHang { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập ngày lập")]
    [DataType(DataType.DateTime)]
    public DateTime NgayLap { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tổng tiền")]
    [Range(0, 9999999999999999.99, ErrorMessage = "Tổng tiền phải lớn hơn hoặc bằng 0")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TongTien { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập phương thức thanh toán")]
    [StringLength(50, ErrorMessage = "Phương thức thanh toán không được vượt quá 50 ký tự")]
    public string PhuongThucThanhToan { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập ghi chú")]
    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
    public string GhiChu { get; set; } = null!;

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        = new List<ChiTietHoaDon>();

    [ForeignKey(nameof(MaKhachHang))]
    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;

    [ForeignKey(nameof(MaNhanVien))]
    public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;
}