using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using quanlydienthoai.Models;

namespace quanlydienthoai.Controllers
{
    public class ChiTietHoaDonController : Controller
    {
        private readonly QuanLyBanDienThoaiContext _context;

        public ChiTietHoaDonController(QuanLyBanDienThoaiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string? mahoadon,
            string? timkiem)
        {
            var danhSach = _context.ChiTietHoaDon
                .Include(x => x.MaHoaDonNavigation)
                .Include(x => x.MaSanPhamNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(mahoadon))
            {
                mahoadon = mahoadon.Trim();

                danhSach = danhSach.Where(x =>
                    x.MaHoaDon == mahoadon);
            }

            if (!string.IsNullOrWhiteSpace(timkiem))
            {
                timkiem = timkiem.Trim();

                danhSach = danhSach.Where(x =>
                    x.MaHoaDon.Contains(timkiem) ||
                    x.MaSanPham.Contains(timkiem) ||
                    x.MaSanPhamNavigation.TenSanPham.Contains(timkiem));
            }

            ViewBag.MaHoaDon = mahoadon;
            ViewBag.TimKiem = timkiem;

            return View(await danhSach
                .OrderBy(x => x.MaHoaDon)
                .ThenBy(x => x.MaSanPham)
                .ToListAsync());
        }

        public async Task<IActionResult> DanhSach(string? timkiem)
        {
            var danhSach = _context.ChiTietHoaDon
                .Include(x => x.MaHoaDonNavigation)
                .Include(x => x.MaSanPhamNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(timkiem))
            {
                timkiem = timkiem.Trim();

                danhSach = danhSach.Where(x =>
                    x.MaHoaDon.Contains(timkiem) ||
                    x.MaSanPham.Contains(timkiem) ||
                    x.MaSanPhamNavigation.TenSanPham.Contains(timkiem));
            }

            ViewBag.MaHoaDon = null;
            ViewBag.TimKiem = timkiem;

            return View(
                "Index",
                await danhSach
                    .OrderBy(x => x.MaHoaDon)
                    .ThenBy(x => x.MaSanPham)
                    .ToListAsync()
            );
        }

        public async Task<IActionResult> Details(
            string? mahoadon,
            string? masanpham)
        {
            if (string.IsNullOrWhiteSpace(mahoadon) ||
                string.IsNullOrWhiteSpace(masanpham))
            {
                return NotFound();
            }

            var chiTietHoaDon = await _context.ChiTietHoaDon
                .Include(x => x.MaHoaDonNavigation)
                .Include(x => x.MaSanPhamNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.MaHoaDon == mahoadon &&
                    x.MaSanPham == masanpham);

            if (chiTietHoaDon == null)
                return NotFound();

            return View(chiTietHoaDon);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string? mahoadon)
        {
            var model = new ChiTietHoaDon();

            if (!string.IsNullOrWhiteSpace(mahoadon))
            {
                model.MaHoaDon = mahoadon;
            }

            await NapDanhSachKhoaNgoai(
                model.MaHoaDon,
                null
            );

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("MaHoaDon,MaSanPham,SoLuong,DonGia")]
            ChiTietHoaDon chiTietHoaDon)
        {
            ModelState.Remove(nameof(chiTietHoaDon.MaHoaDonNavigation));
            ModelState.Remove(nameof(chiTietHoaDon.MaSanPhamNavigation));
            ModelState.Remove(nameof(chiTietHoaDon.ThanhTien));

            if (ModelState.IsValid)
            {
                var hoaDonTonTai = await _context.HoaDon
                    .AnyAsync(x =>
                        x.MaHoaDon == chiTietHoaDon.MaHoaDon);

                if (!hoaDonTonTai)
                {
                    ModelState.AddModelError(
                        nameof(chiTietHoaDon.MaHoaDon),
                        "Hóa đơn không tồn tại."
                    );
                }

                var sanPhamTonTai = await _context.SanPham
                    .AnyAsync(x =>
                        x.MaSanPham == chiTietHoaDon.MaSanPham);

                if (!sanPhamTonTai)
                {
                    ModelState.AddModelError(
                        nameof(chiTietHoaDon.MaSanPham),
                        "Sản phẩm không tồn tại."
                    );
                }

                var daTonTai = await _context.ChiTietHoaDon
                    .AnyAsync(x =>
                        x.MaHoaDon == chiTietHoaDon.MaHoaDon &&
                        x.MaSanPham == chiTietHoaDon.MaSanPham);

                if (daTonTai)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Sản phẩm này đã có trong hóa đơn."
                    );
                }

                if (chiTietHoaDon.SoLuong <= 0)
                {
                    ModelState.AddModelError(
                        nameof(chiTietHoaDon.SoLuong),
                        "Số lượng phải lớn hơn 0."
                    );
                }

                if (chiTietHoaDon.DonGia <= 0)
                {
                    ModelState.AddModelError(
                        nameof(chiTietHoaDon.DonGia),
                        "Đơn giá phải lớn hơn 0."
                    );
                }
            }

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var sanPham = await _context.SanPham.FindAsync(chiTietHoaDon.MaSanPham);
                    if (sanPham == null)
                    {
                        ModelState.AddModelError(nameof(chiTietHoaDon.MaSanPham), "Không tìm thấy sản phẩm.");
                    }
                    else if (sanPham.SoLuong < chiTietHoaDon.SoLuong)
                    {
                        ModelState.AddModelError(
                            nameof(chiTietHoaDon.SoLuong),
                            $"Số lượng sản phẩm trong kho không đủ! (Hiện còn: {sanPham.SoLuong})"
                        );
                    }
                    else
                    {
                        // Trừ số lượng kho
                        sanPham.SoLuong -= chiTietHoaDon.SoLuong;

                        // Thêm chi tiết hóa đơn
                        _context.ChiTietHoaDon.Add(chiTietHoaDon);

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        TempData["Success"] = "Thêm chi tiết hóa đơn thành công.";
                        return RedirectToAction(nameof(DanhSach));
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(
                        string.Empty,
                        "Không thể thêm chi tiết hóa đơn do lỗi hệ thống."
                    );
                }
            }

            await NapDanhSachKhoaNgoai(
                chiTietHoaDon.MaHoaDon,
                chiTietHoaDon.MaSanPham
            );

            return View(chiTietHoaDon);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(
            string? mahoadon,
            string? masanpham)
        {
            if (string.IsNullOrWhiteSpace(mahoadon) ||
                string.IsNullOrWhiteSpace(masanpham))
            {
                return NotFound();
            }

            var chiTietHoaDon = await _context.ChiTietHoaDon
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.MaHoaDon == mahoadon &&
                    x.MaSanPham == masanpham);

            if (chiTietHoaDon == null)
                return NotFound();

            await NapDanhSachKhoaNgoai(
                chiTietHoaDon.MaHoaDon,
                chiTietHoaDon.MaSanPham
            );

            return View(chiTietHoaDon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string? mahoadon,
            string? masanpham,
            [Bind("MaHoaDon,MaSanPham,SoLuong,DonGia")]
            ChiTietHoaDon chiTietHoaDon)
        {
            ModelState.Remove(nameof(chiTietHoaDon.MaHoaDonNavigation));
            ModelState.Remove(nameof(chiTietHoaDon.MaSanPhamNavigation));
            ModelState.Remove(nameof(chiTietHoaDon.ThanhTien));

            if (string.IsNullOrWhiteSpace(mahoadon) ||
                string.IsNullOrWhiteSpace(masanpham))
            {
                return NotFound();
            }

            if (mahoadon != chiTietHoaDon.MaHoaDon ||
                masanpham != chiTietHoaDon.MaSanPham)
            {
                return NotFound();
            }

            var banGhiCu = await _context.ChiTietHoaDon
                .FirstOrDefaultAsync(x =>
                    x.MaHoaDon == mahoadon &&
                    x.MaSanPham == masanpham);

            if (banGhiCu == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                if (chiTietHoaDon.SoLuong <= 0)
                {
                    ModelState.AddModelError(
                        nameof(chiTietHoaDon.SoLuong),
                        "Số lượng phải lớn hơn 0."
                    );
                }

                if (chiTietHoaDon.DonGia <= 0)
                {
                    ModelState.AddModelError(
                        nameof(chiTietHoaDon.DonGia),
                        "Đơn giá phải lớn hơn 0."
                    );
                }
            }

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var sanPham = await _context.SanPham.FindAsync(chiTietHoaDon.MaSanPham);
                    if (sanPham == null)
                    {
                        ModelState.AddModelError(nameof(chiTietHoaDon.MaSanPham), "Sản phẩm không tồn tại.");
                    }
                    else
                    {
                        int chenhLechSoLuong = chiTietHoaDon.SoLuong - banGhiCu.SoLuong;

                        // Nếu tăng số lượng mua -> cần kiểm tra kho có đủ để trừ tiếp không
                        if (chenhLechSoLuong > 0 && sanPham.SoLuong < chenhLechSoLuong)
                        {
                            ModelState.AddModelError(
                                nameof(chiTietHoaDon.SoLuong),
                                $"Số lượng trong kho không đủ để tăng thêm! (Kho còn: {sanPham.SoLuong})"
                            );
                        }
                        else
                        {
                            // Cập nhật lại kho:
                            // Nếu chenhLech > 0 (mua thêm): Kho trừ đi chenhLech
                            // Nếu chenhLech < 0 (giảm mua): Kho cộng lại phần thừa
                            sanPham.SoLuong -= chenhLechSoLuong;

                            banGhiCu.SoLuong = chiTietHoaDon.SoLuong;
                            banGhiCu.DonGia = chiTietHoaDon.DonGia;

                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();

                            TempData["Success"] = "Cập nhật chi tiết hóa đơn thành công.";
                            return RedirectToAction(nameof(DanhSach));
                        }
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(
                        string.Empty,
                        "Không thể cập nhật chi tiết hóa đơn."
                    );
                }
            }

            await NapDanhSachKhoaNgoai(
                chiTietHoaDon.MaHoaDon,
                chiTietHoaDon.MaSanPham
            );

            return View(chiTietHoaDon);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(
            string? mahoadon,
            string? masanpham)
        {
            if (string.IsNullOrWhiteSpace(mahoadon) ||
                string.IsNullOrWhiteSpace(masanpham))
            {
                return NotFound();
            }

            var chiTietHoaDon = await _context.ChiTietHoaDon
                .Include(x => x.MaHoaDonNavigation)
                .Include(x => x.MaSanPhamNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.MaHoaDon == mahoadon &&
                    x.MaSanPham == masanpham);

            if (chiTietHoaDon == null)
                return NotFound();

            return View(chiTietHoaDon);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            string? mahoadon,
            string? masanpham)
        {
            if (string.IsNullOrWhiteSpace(mahoadon) ||
                string.IsNullOrWhiteSpace(masanpham))
            {
                return NotFound();
            }

            var chiTietHoaDon = await _context.ChiTietHoaDon
                .FirstOrDefaultAsync(x =>
                    x.MaHoaDon == mahoadon &&
                    x.MaSanPham == masanpham);

            if (chiTietHoaDon == null)
                return NotFound();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var sanPham = await _context.SanPham.FindAsync(chiTietHoaDon.MaSanPham);
                if (sanPham != null)
                {
                    sanPham.SoLuong += chiTietHoaDon.SoLuong;
                }

                _context.ChiTietHoaDon.Remove(chiTietHoaDon);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Xóa chi tiết hóa đơn thành công và đã hoàn trả tồn kho.";
                return RedirectToAction(nameof(DanhSach));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["DeleteError"] = "Không thể xóa chi tiết hóa đơn.";

                return RedirectToAction(
                    nameof(Delete),
                    new
                    {
                        mahoadon,
                        masanpham
                    });
            }
        }

        private async Task NapDanhSachKhoaNgoai(
            string? maHoaDon = null,
            string? maSanPham = null)
        {
            ViewBag.MaHoaDon = new SelectList(
                await _context.HoaDon
                    .OrderBy(x => x.MaHoaDon)
                    .ToListAsync(),
                "MaHoaDon",
                "MaHoaDon",
                maHoaDon
            );

            ViewBag.MaSanPham = new SelectList(
                await _context.SanPham
                    .OrderBy(x => x.TenSanPham)
                    .ToListAsync(),
                "MaSanPham",
                "TenSanPham",
                maSanPham
            );
        }
    }
}