using System;
using System.ComponentModel.DataAnnotations;

namespace quanlydienthoai.Models;

public partial class TaiKhoan
{
    [Key]
    [StringLength(10)]
    public string MaTaiKhoan { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    [StringLength(
        50,
        MinimumLength = 3,
        ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự")]
    [RegularExpression(
        @"^[a-zA-Z0-9_]+$",
        ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới")]
    public string TenDangNhap { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [StringLength(
        255,
        MinimumLength = 6,
        ErrorMessage = "Mật khẩu phải từ 6 đến 255 ký tự")]
    public string MatKhau { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(
        100,
        ErrorMessage = "Email không được vượt quá 100 ký tự")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn quyền")]
    [RegularExpression(
        @"^(Admin|Quản lý|Nhân viên)$",
        ErrorMessage = "Quyền chỉ được chọn Admin, Quản lý hoặc Nhân viên")]
    [StringLength(
        30,
        ErrorMessage = "Phân quyền không được vượt quá 30 ký tự")]
    public string PhanQuyen { get; set; } = null!;

    public DateTime NgayTao { get; set; }
}