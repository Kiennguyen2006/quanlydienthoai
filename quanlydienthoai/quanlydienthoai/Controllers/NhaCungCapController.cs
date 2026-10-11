using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlydienthoai.Models;

namespace quanlydienthoai.Controllers
{
    public class NhaCungCapController : Controller
    {
        private readonly QuanLyBanDienThoaiContext _context;

        public NhaCungCapController(QuanLyBanDienThoaiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? timkiem)
        {
            var danhSach = _context.NhaCungCap.AsQueryable();

            if (!string.IsNullOrWhiteSpace(timkiem))
            {
                timkiem = timkiem.Trim();

                danhSach = danhSach.Where(x =>
                    x.MaNhaCungCap.Contains(timkiem) ||
                    x.TenNhaCungCap.Contains(timkiem) ||
                    x.SoDienThoai.Contains(timkiem) ||
                    x.Email.Contains(timkiem) ||
                    x.DiaChi.Contains(timkiem) ||
                    x.MaSoThue.Contains(timkiem));
            }

            ViewBag.TimKiem = timkiem;

            return View(await danhSach
                .OrderBy(x => x.MaNhaCungCap)
                .ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? manhacungcap)
        {
            if (string.IsNullOrWhiteSpace(manhacungcap))
                return NotFound();

            var nhacungcap = await _context.NhaCungCap
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaNhaCungCap == manhacungcap);

            if (nhacungcap == null)
                return NotFound();

            return View(nhacungcap);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhaCungCap nhacungcap)
        {
            ModelState.Remove(nameof(nhacungcap.MaNhaCungCap));

            if (!string.IsNullOrWhiteSpace(nhacungcap.SoDienThoai))
            {
                if (await _context.NhaCungCap.AnyAsync(x =>
                    x.SoDienThoai == nhacungcap.SoDienThoai))
                {
                    ModelState.AddModelError(
                        nameof(nhacungcap.SoDienThoai),
                        "Số điện thoại đã tồn tại.");
                }
            }

            if (!string.IsNullOrWhiteSpace(nhacungcap.Email))
            {
                if (await _context.NhaCungCap.AnyAsync(x =>
                    x.Email == nhacungcap.Email))
                {
                    ModelState.AddModelError(
                        nameof(nhacungcap.Email),
                        "Email đã tồn tại.");
                }
            }

            if (!string.IsNullOrWhiteSpace(nhacungcap.MaSoThue))
            {
                if (await _context.NhaCungCap.AnyAsync(x =>
                    x.MaSoThue == nhacungcap.MaSoThue))
                {
                    ModelState.AddModelError(
                        nameof(nhacungcap.MaSoThue),
                        "Mã số thuế đã tồn tại.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(nhacungcap);
            }

            nhacungcap.MaNhaCungCap = await TaoMaNhaCungCap();

            try
            {
                _context.NhaCungCap.Add(nhacungcap);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm nhà cung cấp thành công.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Không thể thêm nhà cung cấp. Vui lòng kiểm tra lại dữ liệu.");

                return View(nhacungcap);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? manhacungcap)
        {
            if (string.IsNullOrWhiteSpace(manhacungcap))
                return NotFound();

            var nhacungcap = await _context.NhaCungCap
                .FirstOrDefaultAsync(x => x.MaNhaCungCap == manhacungcap);

            if (nhacungcap == null)
                return NotFound();

            return View(nhacungcap);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string? manhacungcap,
            NhaCungCap nhacungcap)
        {
            if (string.IsNullOrWhiteSpace(manhacungcap))
                return NotFound();

            if (manhacungcap != nhacungcap.MaNhaCungCap)
                return NotFound();

            ModelState.Remove(nameof(nhacungcap.MaNhaCungCap));

            if (!string.IsNullOrWhiteSpace(nhacungcap.SoDienThoai))
            {
                if (await _context.NhaCungCap.AnyAsync(x =>
                    x.SoDienThoai == nhacungcap.SoDienThoai &&
                    x.MaNhaCungCap != manhacungcap))
                {
                    ModelState.AddModelError(
                        nameof(nhacungcap.SoDienThoai),
                        "Số điện thoại đã tồn tại.");
                }
            }

            if (!string.IsNullOrWhiteSpace(nhacungcap.Email))
            {
                if (await _context.NhaCungCap.AnyAsync(x =>
                    x.Email == nhacungcap.Email &&
                    x.MaNhaCungCap != manhacungcap))
                {
                    ModelState.AddModelError(
                        nameof(nhacungcap.Email),
                        "Email đã tồn tại.");
                }
            }

            if (!string.IsNullOrWhiteSpace(nhacungcap.MaSoThue))
            {
                if (await _context.NhaCungCap.AnyAsync(x =>
                    x.MaSoThue == nhacungcap.MaSoThue &&
                    x.MaNhaCungCap != manhacungcap))
                {
                    ModelState.AddModelError(
                        nameof(nhacungcap.MaSoThue),
                        "Mã số thuế đã tồn tại.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(nhacungcap);
            }

            var nhaCungCapCu = await _context.NhaCungCap
                .FirstOrDefaultAsync(x => x.MaNhaCungCap == manhacungcap);

            if (nhaCungCapCu == null)
                return NotFound();

            nhaCungCapCu.TenNhaCungCap = nhacungcap.TenNhaCungCap;
            nhaCungCapCu.SoDienThoai = nhacungcap.SoDienThoai;
            nhaCungCapCu.Email = nhacungcap.Email;
            nhaCungCapCu.DiaChi = nhacungcap.DiaChi;
            nhaCungCapCu.MaSoThue = nhacungcap.MaSoThue;
            nhaCungCapCu.GhiChu = nhacungcap.GhiChu;

            try
            {
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật nhà cung cấp thành công.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Không thể cập nhật nhà cung cấp.");

                return View(nhacungcap);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? manhacungcap)
        {
            if (string.IsNullOrWhiteSpace(manhacungcap))
                return NotFound();

            var nhacungcap = await _context.NhaCungCap
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaNhaCungCap == manhacungcap);

            if (nhacungcap == null)
                return NotFound();

            return View(nhacungcap);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? manhacungcap)
        {
            if (string.IsNullOrWhiteSpace(manhacungcap))
                return NotFound();

            var nhacungcap = await _context.NhaCungCap
                .FirstOrDefaultAsync(x => x.MaNhaCungCap == manhacungcap);

            if (nhacungcap == null)
                return NotFound();

            try
            {
                _context.NhaCungCap.Remove(nhacungcap);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa nhà cung cấp thành công.";
            }
            catch (DbUpdateException)
            {
                TempData["DeleteError"] =
                    "Không thể xóa nhà cung cấp vì đang có sản phẩm liên quan.";

            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<string> TaoMaNhaCungCap()
        {
            var danhSach = await _context.NhaCungCap
                .Select(x => x.MaNhaCungCap)
                .ToListAsync();

            int soLonNhat = 0;

            foreach (var ma in danhSach)
            {
                if (string.IsNullOrWhiteSpace(ma))
                    continue;

                if (ma.StartsWith("NCC") &&
                    int.TryParse(ma.Substring(3), out int so))
                {
                    if (so > soLonNhat)
                        soLonNhat = so;
                }
            }

            return "NCC" + (soLonNhat + 1).ToString("D3");
        }
    }
}