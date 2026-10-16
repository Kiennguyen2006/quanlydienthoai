using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlydienthoai.Models;

public partial class ChiTietHoaDon
{
    [Required(ErrorMessage = "Vui lòng chọn hóa đơn")]
    [StringLength(10, ErrorMessage = "Mã hóa đơn không được vượt quá 10 ký tự")]
    public string MaHoaDon { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
    [StringLength(10, ErrorMessage = "Mã sản phẩm không được vượt quá 10 ký tự")]
    public string MaSanPham { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập số lượng")]
    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải là số nguyên lớn hơn 0")]
    public int SoLuong { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập đơn giá")]
    [Range(0.01, 9999999999999999.99,
        ErrorMessage = "Đơn giá phải lớn hơn 0")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal DonGia { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ThanhTien { get; set; }

    public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}