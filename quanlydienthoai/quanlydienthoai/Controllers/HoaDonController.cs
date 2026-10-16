using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using quanlydienthoai.Models;

public class HoaDonController : Controller
{
    private readonly QuanLyBanDienThoaiContext _context;

    public HoaDonController(QuanLyBanDienThoaiContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? timkiem)
    {
        var danhSach = _context.HoaDon
            .Include(x => x.MaNhanVienNavigation)
            .Include(x => x.MaKhachHangNavigation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(timkiem))
        {
            timkiem = timkiem.Trim();

            danhSach = danhSach.Where(x =>
                x.MaHoaDon.Contains(timkiem) ||
                x.MaNhanVien.Contains(timkiem) ||
                x.MaKhachHang.Contains(timkiem) ||
                x.MaNhanVienNavigation.HoTen.Contains(timkiem) ||
                x.MaKhachHangNavigation.HoTen.Contains(timkiem) ||
                x.PhuongThucThanhToan.Contains(timkiem));
        }

        ViewBag.TimKiem = timkiem;

        return View(await danhSach
            .OrderByDescending(x => x.NgayLap)
            .ToListAsync());
    }

    public async Task<IActionResult> Details(string? mahoadon)
    {
        if (string.IsNullOrWhiteSpace(mahoadon))
            return NotFound();

        var hoadon = await _context.HoaDon
            .Include(x => x.MaNhanVienNavigation)
            .Include(x => x.MaKhachHangNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaHoaDon == mahoadon);

        if (hoadon == null)
            return NotFound();

        return View(hoadon);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await NapDanhSachKhoaNgoai();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("MaHoaDon,MaNhanVien,MaKhachHang,NgayLap,TongTien,PhuongThucThanhToan,GhiChu")]
        HoaDon hoadon)
    {
        ModelState.Remove(nameof(hoadon.MaHoaDon));
        ModelState.Remove(nameof(hoadon.MaKhachHangNavigation));
        ModelState.Remove(nameof(hoadon.MaNhanVienNavigation));
        ModelState.Remove(nameof(hoadon.ChiTietHoaDons));

        if (ModelState.IsValid)
        {
            try
            {
                hoadon.MaHoaDon = await TaoMa(
                    "HD",
                    _context.HoaDon.Select(x => x.MaHoaDon)
                );

                _context.HoaDon.Add(hoadon);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm hóa đơn thành công.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Không thể lưu hóa đơn. Hãy kiểm tra khách hàng, nhân viên và dữ liệu đã nhập."
                );
            }
        }

        await NapDanhSachKhoaNgoai(
            hoadon.MaKhachHang,
            hoadon.MaNhanVien
        );

        return View(hoadon);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? mahoadon)
    {
        if (string.IsNullOrWhiteSpace(mahoadon))
            return NotFound();

        var hoadon = await _context.HoaDon
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaHoaDon == mahoadon);

        if (hoadon == null)
            return NotFound();

        await NapDanhSachKhoaNgoai(
            hoadon.MaKhachHang,
            hoadon.MaNhanVien
        );

        return View(hoadon);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        string? mahoadon,
        [Bind("MaHoaDon,MaNhanVien,MaKhachHang,NgayLap,TongTien,PhuongThucThanhToan,GhiChu")]
        HoaDon hoadon)
    {
        ModelState.Remove(nameof(hoadon.MaKhachHangNavigation));
        ModelState.Remove(nameof(hoadon.MaNhanVienNavigation));
        ModelState.Remove(nameof(hoadon.ChiTietHoaDons));

        if (string.IsNullOrWhiteSpace(mahoadon))
            return NotFound();

        if (mahoadon != hoadon.MaHoaDon)
            return NotFound();

        var banGhi = await _context.HoaDon
            .FirstOrDefaultAsync(x => x.MaHoaDon == mahoadon);

        if (banGhi == null)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                banGhi.MaNhanVien = hoadon.MaNhanVien;
                banGhi.MaKhachHang = hoadon.MaKhachHang;
                banGhi.NgayLap = hoadon.NgayLap;
                banGhi.TongTien = hoadon.TongTien;
                banGhi.PhuongThucThanhToan = hoadon.PhuongThucThanhToan;
                banGhi.GhiChu = hoadon.GhiChu;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật hóa đơn thành công.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Không thể cập nhật hóa đơn. Hãy kiểm tra lại dữ liệu."
                );
            }
        }

        await NapDanhSachKhoaNgoai(
            hoadon.MaKhachHang,
            hoadon.MaNhanVien
        );

        return View(hoadon);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string? mahoadon)
    {
        if (string.IsNullOrWhiteSpace(mahoadon))
            return NotFound();

        var hoadon = await _context.HoaDon
            .Include(x => x.MaNhanVienNavigation)
            .Include(x => x.MaKhachHangNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaHoaDon == mahoadon);

        if (hoadon == null)
            return NotFound();

        return View(hoadon);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? mahoadon)
    {
        if (string.IsNullOrWhiteSpace(mahoadon))
            return NotFound();

        var hoadon = await _context.HoaDon
            .FirstOrDefaultAsync(x => x.MaHoaDon == mahoadon);

        if (hoadon == null)
            return NotFound();

        try
        {
            _context.HoaDon.Remove(hoadon);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa hóa đơn thành công.";
        }
        catch (DbUpdateException)
        {
            TempData["DeleteError"] =
                "Không thể xóa hóa đơn vì hóa đơn đang có chi tiết hóa đơn.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task NapDanhSachKhoaNgoai(
        string? maKhachHang = null,
        string? maNhanVien = null)
    {
        ViewBag.MaKhachHang = new SelectList(
            await _context.KhachHang
                .OrderBy(x => x.HoTen)
                .ToListAsync(),
            "MaKhachHang",
            "HoTen",
            maKhachHang
        );

        ViewBag.MaNhanVien = new SelectList(
            await _context.NhanVien
                .OrderBy(x => x.HoTen)
                .ToListAsync(),
            "MaNhanVien",
            "HoTen",
            maNhanVien
        );
    }

    private async Task<string> TaoMa(
        string tienTo,
        IQueryable<string> cacMa)
    {
        var danhSachMa = await cacMa.ToListAsync();

        int soLonNhat = danhSachMa
            .Where(ma =>
                !string.IsNullOrWhiteSpace(ma) &&
                ma.StartsWith(tienTo))
            .Select(ma =>
            {
                var phanSo = ma.Substring(tienTo.Length);

                return int.TryParse(phanSo, out int so)
                    ? so
                    : 0;
            })
            .DefaultIfEmpty(0)
            .Max();

        int soTiepTheo = soLonNhat + 1;

        string maMoi;

        do
        {
            maMoi = tienTo + soTiepTheo.ToString("D3");
            soTiepTheo++;
        }
        while (await cacMa.AnyAsync(ma => ma == maMoi));

        return maMoi;
    }
}