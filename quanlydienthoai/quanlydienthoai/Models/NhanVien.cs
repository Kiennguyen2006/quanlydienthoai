using System.ComponentModel.DataAnnotations;

namespace quanlydienthoai.Models;

public partial class NhanVien
{
    [Key]
    public string MaNhanVien { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
    public string HoTen { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
    public DateTime? NgaySinh { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn giới tính")]
    public string GioiTinh { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải có 10 số và bắt đầu bằng số 0")]
    public string SoDienThoai { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
    [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
    public string DiaChi { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập chức vụ")]
    [StringLength(50, ErrorMessage = "Chức vụ không được vượt quá 50 ký tự")]
    public string ChucVu { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn ngày vào làm")]
    public DateTime? NgayVaoLam { get; set; }

    public string? Anh { get; set; }

    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
    public string? MoTa { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}