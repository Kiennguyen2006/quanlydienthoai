using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlydienthoai.Models;

public partial class SanPham
{
    [Key]
    [StringLength(10)]
    public string MaSanPham { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
    [StringLength(10)]
    public string MaNhaCungCap { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
    [StringLength(150, ErrorMessage = "Tên sản phẩm không được vượt quá 150 ký tự")]
    public string TenSanPham { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập hãng sản xuất")]
    [StringLength(100, ErrorMessage = "Hãng sản xuất không được vượt quá 100 ký tự")]
    public string HangSanXuat { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập Model")]
    [StringLength(100, ErrorMessage = "Model không được vượt quá 100 ký tự")]
    public string Model { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập giá nhập")]
    [Range(
        0,
        9999999999999999.99,
        ErrorMessage = "Giá nhập phải lớn hơn hoặc bằng 0")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal GiaNhap { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập giá bán")]
    [Range(
        0,
        9999999999999999.99,
        ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal GiaBan { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số lượng")]
    [Range(
        0,
        int.MaxValue,
        ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
    public int SoLuong { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập màu sắc")]
    [StringLength(50, ErrorMessage = "Màu sắc không được vượt quá 50 ký tự")]
    public string MauSac { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập RAM")]
    [StringLength(20, ErrorMessage = "RAM không được vượt quá 20 ký tự")]
    public string Ram { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập ROM")]
    [StringLength(20, ErrorMessage = "ROM không được vượt quá 20 ký tự")]
    public string Rom { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập Chip")]
    [StringLength(100, ErrorMessage = "Chip không được vượt quá 100 ký tự")]
    public string Chip { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập CPU")]
    [StringLength(150, ErrorMessage = "CPU không được vượt quá 150 ký tự")]
    public string Cpu { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập GPU")]
    [StringLength(150, ErrorMessage = "GPU không được vượt quá 150 ký tự")]
    public string Gpu { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập thông tin màn hình")]
    [StringLength(200, ErrorMessage = "Màn hình không được vượt quá 200 ký tự")]
    public string ManHinh { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập thông tin Camera")]
    [StringLength(200, ErrorMessage = "Camera không được vượt quá 200 ký tự")]
    public string Camera { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập thông tin Pin")]
    [StringLength(100, ErrorMessage = "Thông tin pin không được vượt quá 100 ký tự")]
    public string Pin { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập hệ điều hành")]
    [StringLength(100, ErrorMessage = "Hệ điều hành không được vượt quá 100 ký tự")]
    public string HeDieuHanh { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập tên ảnh")]
    [StringLength(255, ErrorMessage = "Tên ảnh không được vượt quá 255 ký tự")]
    public string Anh { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập mô tả")]
    public string MoTa { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        = new List<ChiTietHoaDon>();

    public virtual NhaCungCap MaNhaCungCapNavigation { get; set; } = null!;
}