using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlydienthoai.Models;

namespace quanlydienthoai.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly QuanLyBanDienThoaiContext _context;

        public KhachHangController(QuanLyBanDienThoaiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? timkiem)
        {
            var danhSach = _context.KhachHang.AsQueryable();

            if (!string.IsNullOrWhiteSpace(timkiem))
            {
                danhSach = danhSach.Where(x =>
                    (x.MaKhachHang != null && x.MaKhachHang.Contains(timkiem)) ||
                    x.HoTen.Contains(timkiem));
            }

            ViewBag.TimKiem = timkiem;

            return View(await danhSach.ToListAsync());
        }

        public async Task<IActionResult> Details(string? makhachhang)
        {
            if (makhachhang == null)
            {
                return NotFound();
            }

            var khachhang = await _context.KhachHang
                .FirstOrDefaultAsync(x => x.MaKhachHang == makhachhang);

            if (khachhang == null)
            {
                return NotFound();
            }

            return View(khachhang);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang khachhang)
        {
            ModelState.Remove(nameof(khachhang.MaKhachHang));
            ModelState.Remove(nameof(khachhang.NgayTao));
            ModelState.Remove(nameof(khachhang.HoaDons));
            khachhang.HoaDons = new List<HoaDon>();
            khachhang.MaKhachHang = await TaoMaKhachHang();
            khachhang.NgayTao = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    while (await _context.KhachHang.AnyAsync(x => x.MaKhachHang == khachhang.MaKhachHang))
                    {
                        khachhang.MaKhachHang = await TaoMaKhachHang();
                    }

                    _context.KhachHang.Add(khachhang);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty,
                        "Không thể lưu khách hàng. Hãy kiểm tra số điện thoại đã tồn tại và thử lại.");
                }
            }

            return View(khachhang);
        }

        private async Task<string> TaoMaKhachHang()
        {
            var danhSachMa = await _context.KhachHang
                .Select(x => x.MaKhachHang)
                .ToListAsync();

            var soLonNhat = danhSachMa
                .Where(ma => !string.IsNullOrWhiteSpace(ma))
                .Select(ma =>
                {
                    var maHienTai = ma!;
                    var phanChu = new string(maHienTai.TakeWhile(char.IsLetter).ToArray());
                    var phanSo = new string(maHienTai.SkipWhile(char.IsLetter).ToArray());
                    return phanChu == "KH" && int.TryParse(phanSo, out var so) ? so : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            return "KH" + (soLonNhat + 1).ToString("D3");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? makhachhang)
        {
            if (makhachhang == null)
            {
                return NotFound();
            }

            var khachhang = await _context.KhachHang
                .FindAsync(makhachhang);

            if (khachhang == null)
            {
                return NotFound();
            }

            return View(khachhang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string makhachhang,
            KhachHang khachhang)
        {
            if (makhachhang == null || makhachhang != khachhang.MaKhachHang)
            {
                return NotFound();
            }

            var banGhi = await _context.KhachHang.FindAsync(makhachhang);
            if (banGhi == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    banGhi.HoTen = khachhang.HoTen;
                    banGhi.GioiTinh = khachhang.GioiTinh;
                    banGhi.SoDienThoai = khachhang.SoDienThoai;
                    banGhi.Email = khachhang.Email;
                    banGhi.DiaChi = khachhang.DiaChi;
                    banGhi.GhiChu = khachhang.GhiChu;
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty,
                        "Unable to save the customer. Check whether the phone number is already registered.");
                }
            }

            return View(khachhang);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? makhachhang)
        {
            if (makhachhang == null)
            {
                return NotFound();
            }

            var khachhang = await _context.KhachHang
                .FirstOrDefaultAsync(x => x.MaKhachHang == makhachhang);

            if (khachhang == null)
            {
                return NotFound();
            }

            return View(khachhang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string makhachhang)
        {
            var khachhang = await _context.KhachHang
                .FindAsync(makhachhang);

            if (khachhang != null)
            {
                _context.KhachHang.Remove(khachhang);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    TempData["KhachHangDeleteError"] = "Không thể xóa vì dữ liệu đang được sử dụng.";
                    return RedirectToAction(nameof(Delete), new { makhachhang });
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
