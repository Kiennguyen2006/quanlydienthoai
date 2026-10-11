using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace quanlydienthoai.Models;

public partial class QuanLyBanDienThoaiContext : DbContext
{
    public QuanLyBanDienThoaiContext()
    {
    }

    public QuanLyBanDienThoaiContext(DbContextOptions<QuanLyBanDienThoaiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDon { get; set; }

    public virtual DbSet<HoaDon> HoaDon { get; set; }

    public virtual DbSet<KhachHang> KhachHang { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCap { get; set; }

    public virtual DbSet<NhanVien> NhanVien { get; set; }

    public virtual DbSet<SanPham> SanPham { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoan { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => new { e.MaHoaDon, e.MaSanPham });

            entity.ToTable("ChiTietHoaDon");

            entity.Property(e => e.MaHoaDon)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.MaSanPham)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.DonGia)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.ThanhTien)
                .HasComputedColumnSql("([SoLuong]*[DonGia])", true)
                .HasColumnType("decimal(29, 2)");

            entity.HasOne(d => d.MaHoaDonNavigation)
                .WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaHoaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietHoaDon_HoaDon");

            entity.HasOne(d => d.MaSanPhamNavigation)
                .WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietHoaDon_SanPham");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon)
                .HasName("PK__HoaDon__835ED13B008ED9F6");

            entity.ToTable("HoaDon", tb =>
                tb.HasTrigger("TR_HoaDon_TuDongTaoMa"));

            entity.Property(e => e.MaHoaDon)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.GhiChu)
                .HasMaxLength(500);

            entity.Property(e => e.MaKhachHang)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.NgayLap)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.PhuongThucThanhToan)
                .HasMaxLength(50);

            entity.Property(e => e.TongTien)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaKhachHangNavigation)
                .WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDon_KhachHang");

            entity.HasOne(d => d.MaNhanVienNavigation)
                .WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNhanVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDon_NhanVien");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang)
                .HasName("PK__KhachHan__88D2F0E5653D5C0B");

            entity.ToTable("KhachHang", tb =>
                tb.HasTrigger("TR_KhachHang_TuDongTaoMa"));

            entity.HasIndex(e => e.SoDienThoai,
                "UQ__KhachHan__0389B7BDB5CAB343")
                .IsUnique();

            entity.Property(e => e.MaKhachHang)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.DiaChi)
                .HasMaxLength(255);

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.GhiChu)
                .HasMaxLength(500);

            entity.Property(e => e.GioiTinh)
                .HasMaxLength(10);

            entity.Property(e => e.HoTen)
                .HasMaxLength(100);

            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNhaCungCap)
                .HasName("PK__NhaCungC__53DA9205B42BDDB4");

            entity.ToTable("NhaCungCap", tb =>
                tb.HasTrigger("TR_NhaCungCap_TuDongTaoMa"));

            entity.Property(e => e.MaNhaCungCap)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.DiaChi)
                .HasMaxLength(255);

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.GhiChu)
                .HasMaxLength(500);

            entity.Property(e => e.MaSoThue)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.TenNhaCungCap)
                .HasMaxLength(150);
        });


        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham)
                .HasName("PK__SanPham__FAC7442D01A3C3E1");

            entity.ToTable("SanPham", tb =>
                tb.HasTrigger("TR_SanPham_TuDongTaoMa"));

            entity.Property(e => e.MaSanPham)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.Anh)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.Camera)
                .HasMaxLength(200);

            entity.Property(e => e.Chip)
                .HasMaxLength(100);

            entity.Property(e => e.Cpu)
                .HasMaxLength(150)
                .HasColumnName("CPU");

            entity.Property(e => e.GiaBan)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.GiaNhap)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Gpu)
                .HasMaxLength(150)
                .HasColumnName("GPU");

            entity.Property(e => e.HangSanXuat)
                .HasMaxLength(100);

            entity.Property(e => e.HeDieuHanh)
                .HasMaxLength(100);

            entity.Property(e => e.MaNhaCungCap)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.ManHinh)
                .HasMaxLength(200);

            entity.Property(e => e.MauSac)
                .HasMaxLength(50);

            entity.Property(e => e.Model)
                .HasMaxLength(100);

            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.Pin)
                .HasMaxLength(100);

            entity.Property(e => e.Ram)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("RAM");

            entity.Property(e => e.Rom)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ROM");

            entity.Property(e => e.TenSanPham)
                .HasMaxLength(150);

            entity.HasOne(d => d.MaNhaCungCapNavigation)
                .WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaNhaCungCap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPham_NhaCungCap");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan)
                .HasName("PK__TaiKhoan__AD7C6529D05B7893");

            entity.ToTable("TaiKhoan", tb =>
                tb.HasTrigger("TR_TaiKhoan_TuDongTaoMa"));

            entity.HasIndex(e => e.TenDangNhap,
                "UQ__TaiKhoan__55F68FC0F28E83D9")
                .IsUnique();

            entity.HasIndex(e => e.Email,
                "UQ__TaiKhoan__A9D10534E4CA5E58")
                .IsUnique();

            entity.Property(e => e.MaTaiKhoan)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.PhanQuyen)
                .HasMaxLength(30)
                .HasDefaultValue("Nhân viên");

            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}