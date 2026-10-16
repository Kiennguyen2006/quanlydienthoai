using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using quanlydienthoai.Models;

namespace quanlydienthoai.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly QuanLyBanDienThoaiContext _context;
        private readonly IWebHostEnvironment _environment;

        public SanPhamController(
            QuanLyBanDienThoaiContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string? timkiem)
        {
            var danhSach = _context.SanPham
                .Include(x => x.MaNhaCungCapNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(timkiem))
            {
                timkiem = timkiem.Trim();

                danhSach = danhSach.Where(x =>
                    x.MaSanPham.Contains(timkiem) ||
                    x.MaNhaCungCap.Contains(timkiem) ||
                    x.TenSanPham.Contains(timkiem) ||
                    x.HangSanXuat.Contains(timkiem) ||
                    x.Model.Contains(timkiem) ||
                    x.Ram.Contains(timkiem) ||
                    x.Rom.Contains(timkiem) ||
                    x.HeDieuHanh.Contains(timkiem));
            }

            ViewBag.TimKiem = timkiem;

            return View(await danhSach
                .OrderBy(x => x.MaSanPham)
                .ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? masanpham)
        {
            if (string.IsNullOrWhiteSpace(masanpham))
                return NotFound();

            var sanpham = await _context.SanPham
                .Include(x => x.MaNhaCungCapNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaSanPham == masanpham);

            if (sanpham == null)
                return NotFound();

            return View(sanpham);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await NapDanhSachNhaCungCap();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            SanPham sanpham,
            IFormFile? fileAnh)
        {
            ModelState.Remove(nameof(sanpham.MaSanPham));
            ModelState.Remove(nameof(sanpham.NgayTao));
            ModelState.Remove(nameof(sanpham.Anh));
            ModelState.Remove(nameof(sanpham.MaNhaCungCapNavigation));
            ModelState.Remove(nameof(sanpham.ChiTietHoaDons));

            if (fileAnh == null || fileAnh.Length == 0)
            {
                ModelState.AddModelError(
                    nameof(fileAnh),
                    "Vui lòng chọn ảnh sản phẩm.");
            }
            else if (!KiemTraAnh(fileAnh))
            {
                ModelState.AddModelError(
                    nameof(fileAnh),
                    "Ảnh phải có định dạng JPG, JPEG, PNG hoặc WEBP và dung lượng không quá 20MB.");
            }

            if (!ModelState.IsValid)
            {
                await NapDanhSachNhaCungCap(sanpham.MaNhaCungCap);
                return View(sanpham);
            }

            sanpham.MaSanPham = await TaoMaSanPham();
            sanpham.NgayTao = DateTime.Now;

            try
            {
                sanpham.Anh = await LuuAnh(fileAnh!);

                _context.SanPham.Add(sanpham);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm sản phẩm thành công.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Không thể thêm sản phẩm. Vui lòng kiểm tra lại dữ liệu.");

                await NapDanhSachNhaCungCap(sanpham.MaNhaCungCap);

                return View(sanpham);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? masanpham)
        {
            if (string.IsNullOrWhiteSpace(masanpham))
                return NotFound();

            var sanpham = await _context.SanPham
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaSanPham == masanpham);

            if (sanpham == null)
                return NotFound();

            await NapDanhSachNhaCungCap(sanpham.MaNhaCungCap);

            return View(sanpham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string? masanpham,
            SanPham sanpham,
            IFormFile? fileAnh)
        {
            if (string.IsNullOrWhiteSpace(masanpham))
                return NotFound();

            if (masanpham != sanpham.MaSanPham)
                return NotFound();

            ModelState.Remove(nameof(sanpham.MaSanPham));
            ModelState.Remove(nameof(sanpham.NgayTao));
            ModelState.Remove(nameof(sanpham.Anh));
            ModelState.Remove(nameof(sanpham.MaNhaCungCapNavigation));
            ModelState.Remove(nameof(sanpham.ChiTietHoaDons));

            var banGhi = await _context.SanPham
                .FirstOrDefaultAsync(x => x.MaSanPham == masanpham);

            if (banGhi == null)
                return NotFound();

            if (fileAnh != null && fileAnh.Length > 0)
            {
                if (!KiemTraAnh(fileAnh))
                {
                    ModelState.AddModelError(
                        nameof(fileAnh),
                        "Ảnh phải có định dạng JPG, JPEG, PNG hoặc WEBP và dung lượng không quá 20MB.");
                }
            }

            if (!ModelState.IsValid)
            {
                sanpham.Anh = banGhi.Anh;

                await NapDanhSachNhaCungCap(sanpham.MaNhaCungCap);

                return View(sanpham);
            }

            banGhi.MaNhaCungCap = sanpham.MaNhaCungCap;
            banGhi.TenSanPham = sanpham.TenSanPham;
            banGhi.HangSanXuat = sanpham.HangSanXuat;
            banGhi.Model = sanpham.Model;
            banGhi.GiaNhap = sanpham.GiaNhap;
            banGhi.GiaBan = sanpham.GiaBan;
            banGhi.SoLuong = sanpham.SoLuong;
            banGhi.MauSac = sanpham.MauSac;
            banGhi.Ram = sanpham.Ram;
            banGhi.Rom = sanpham.Rom;
            banGhi.Chip = sanpham.Chip;
            banGhi.Cpu = sanpham.Cpu;
            banGhi.Gpu = sanpham.Gpu;
            banGhi.ManHinh = sanpham.ManHinh;
            banGhi.Camera = sanpham.Camera;
            banGhi.Pin = sanpham.Pin;
            banGhi.HeDieuHanh = sanpham.HeDieuHanh;
            banGhi.MoTa = sanpham.MoTa;

            try
            {
                if (fileAnh != null && fileAnh.Length > 0)
                {
                    var anhCu = banGhi.Anh;

                    banGhi.Anh = await LuuAnh(fileAnh);

                    XoaAnhCu(anhCu);
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật sản phẩm thành công.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Không thể cập nhật sản phẩm.");

                sanpham.Anh = banGhi.Anh;

                await NapDanhSachNhaCungCap(sanpham.MaNhaCungCap);

                return View(sanpham);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? masanpham)
        {
            if (string.IsNullOrWhiteSpace(masanpham))
                return NotFound();

            var sanpham = await _context.SanPham
                .Include(x => x.MaNhaCungCapNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaSanPham == masanpham);

            if (sanpham == null)
                return NotFound();

            return View(sanpham);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? masanpham)
        {
            if (string.IsNullOrWhiteSpace(masanpham))
                return NotFound();

            var sanpham = await _context.SanPham
                .FirstOrDefaultAsync(x => x.MaSanPham == masanpham);

            if (sanpham == null)
                return NotFound();

            try
            {
                _context.SanPham.Remove(sanpham);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa sản phẩm thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["DeleteError"] = "Không thể xóa sản phẩm vì sản phẩm đang có trong chi tiết hóa đơn.";
                return RedirectToAction(nameof(Delete), new { masanpham });
            }
        }

        private async Task NapDanhSachNhaCungCap(
            string? maDangChon = null)
        {
            ViewBag.MaNhaCungCap = new SelectList(
                await _context.NhaCungCap
                    .OrderBy(x => x.TenNhaCungCap)
                    .ToListAsync(),
                "MaNhaCungCap",
                "TenNhaCungCap",
                maDangChon);
        }

        private async Task<string> TaoMaSanPham()
        {
            var danhSach = await _context.SanPham
                .Select(x => x.MaSanPham)
                .ToListAsync();

            int soLonNhat = 0;

            foreach (var ma in danhSach)
            {
                if (string.IsNullOrWhiteSpace(ma))
                    continue;

                if (ma.StartsWith("SP") &&
                    int.TryParse(ma.Substring(2), out int so))
                {
                    if (so > soLonNhat)
                        soLonNhat = so;
                }
            }

            return "SP" + (soLonNhat + 1).ToString("D3");
        }

        private bool KiemTraAnh(IFormFile file)
        {
            var duoiFile = Path.GetExtension(file.FileName)
                .ToLowerInvariant();

            var duoiHopLe = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            if (!duoiHopLe.Contains(duoiFile))
                return false;

            if (file.Length > 20 * 1024 * 1024)
                return false;

            return true;
        }

        private async Task<string> LuuAnh(IFormFile file)
        {
            var thuMucImages = Path.Combine(
                _environment.WebRootPath,
                "images");

            if (!Directory.Exists(thuMucImages))
                Directory.CreateDirectory(thuMucImages);

            var duoiFile = Path.GetExtension(file.FileName)
                .ToLowerInvariant();

            var tenFile = Guid.NewGuid().ToString("N") + duoiFile;

            var duongDan = Path.Combine(
                thuMucImages,
                tenFile);

            using (var stream = new FileStream(
                duongDan,
                FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return tenFile;
        }

        private void XoaAnhCu(string? tenFile)
        {
            if (string.IsNullOrWhiteSpace(tenFile))
                return;

            var duongDan = Path.Combine(
                _environment.WebRootPath,
                "images",
                tenFile);

            if (System.IO.File.Exists(duongDan))
            {
                try
                {
                    System.IO.File.Delete(duongDan);
                }
                catch
                {
                }
            }
        }
    }
}