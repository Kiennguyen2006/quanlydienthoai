using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace quanlydienthoai.Models;

public partial class NhaCungCap
{
    [Key]
    [StringLength(10)]
    public string MaNhaCungCap { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập tên nhà cung cấp")]
    [StringLength(150, ErrorMessage = "Tên nhà cung cấp không được vượt quá 150 ký tự")]
    public string TenNhaCungCap { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [RegularExpression(
        @"^0[0-9]{9}$",
        ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số và bắt đầu bằng số 0")]
    [StringLength(10, ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số")]
    public string SoDienThoai { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
    [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
    public string DiaChi { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập mã số thuế")]
    [StringLength(30, ErrorMessage = "Mã số thuế không được vượt quá 30 ký tự")]
    [RegularExpression(
        @"^[0-9]+$",
        ErrorMessage = "Mã số thuế chỉ được chứa chữ số")]
    public string MaSoThue { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập ghi chú")]
    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
    public string GhiChu { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; }
        = new List<SanPham>();
}