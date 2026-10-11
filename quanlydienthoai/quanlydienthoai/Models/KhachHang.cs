using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace quanlydienthoai.Models;

public partial class KhachHang
{
    [Key]
    [StringLength(10)]
    public string? MaKhachHang { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
    public string HoTen { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn giới tính")]
    [RegularExpression(
        @"^(Nam|Nữ|Khác)$",
        ErrorMessage = "Giới tính chỉ được chọn Nam, Nữ hoặc Khác")]
    public string GioiTinh { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [RegularExpression(
        @"^0[0-9]{9}$",
        ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số và bắt đầu bằng số 0")]
    public string SoDienThoai { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
    [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
    public string DiaChi { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập ghi chú")]
    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
    public string GhiChu { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}