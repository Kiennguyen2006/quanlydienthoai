using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlydienthoai.Models;

namespace quanlydienthoai.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly QuanLyBanDienThoaiContext _context;

        public TaiKhoanController(QuanLyBanDienThoaiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? timkiem)
        {
            var danhSach = _context.TaiKhoan.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(timkiem))
            {
                timkiem = timkiem.Trim();

                danhSach = danhSach.Where(x =>
                    x.MaTaiKhoan.Contains(timkiem) ||
                    x.TenDangNhap.Contains(timkiem) ||
                    x.Email.Contains(timkiem) ||
                    x.PhanQuyen.Contains(timkiem));
            }

            ViewBag.TimKiem = timkiem;

            var ketQua = await danhSach
                .OrderBy(x => x.MaTaiKhoan)
                .ToListAsync();

            return View(ketQua);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? mataikhoan)
        {
            if (string.IsNullOrWhiteSpace(mataikhoan))
                return NotFound();

            var taikhoan = await _context.TaiKhoan
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaTaiKhoan == mataikhoan);

            if (taikhoan == null)
                return NotFound();

            return View(taikhoan);
        }

        [HttpGet]
        public IActionResult Create()
        {
            TaoDanhSachPhanQuyen();

            return View(new TaiKhoan
            {
                PhanQuyen = "Nhân viên"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiKhoan taikhoan)
        {
            ModelState.Remove(nameof(taikhoan.MaTaiKhoan));
            ModelState.Remove(nameof(taikhoan.NgayTao));

            if (!string.IsNullOrWhiteSpace(taikhoan.TenDangNhap))
            {
                taikhoan.TenDangNhap = taikhoan.TenDangNhap.Trim();
            }

            if (!string.IsNullOrWhiteSpace(taikhoan.Email))
            {
                taikhoan.Email = taikhoan.Email.Trim();
            }

            if (await _context.TaiKhoan.AnyAsync(x =>
                x.TenDangNhap == taikhoan.TenDangNhap))
            {
                ModelState.AddModelError(
                    nameof(taikhoan.TenDangNhap),
                    "Tên đăng nhập đã tồn tại.");
            }

            if (await _context.TaiKhoan.AnyAsync(x =>
                x.Email == taikhoan.Email))
            {
                ModelState.AddModelError(
                    nameof(taikhoan.Email),
                    "Email đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                TaoDanhSachPhanQuyen(taikhoan.PhanQuyen);

                return View(taikhoan);
            }

            taikhoan.MaTaiKhoan = await TaoMaTaiKhoan();
            taikhoan.NgayTao = DateTime.Now;

            try
            {
                _context.TaiKhoan.Add(taikhoan);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm tài khoản thành công.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Không thể thêm tài khoản. Vui lòng kiểm tra lại dữ liệu.");

                TaoDanhSachPhanQuyen(taikhoan.PhanQuyen);

                return View(taikhoan);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? mataikhoan)
        {
            if (string.IsNullOrWhiteSpace(mataikhoan))
                return NotFound();

            var taikhoan = await _context.TaiKhoan
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaTaiKhoan == mataikhoan);

            if (taikhoan == null)
                return NotFound();

            TaoDanhSachPhanQuyen(taikhoan.PhanQuyen);

            return View(taikhoan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string? mataikhoan,
            TaiKhoan taikhoan)
        {
            if (string.IsNullOrWhiteSpace(mataikhoan))
                return NotFound();

            if (mataikhoan != taikhoan.MaTaiKhoan)
                return NotFound();

            ModelState.Remove(nameof(taikhoan.MaTaiKhoan));
            ModelState.Remove(nameof(taikhoan.NgayTao));

            if (!string.IsNullOrWhiteSpace(taikhoan.TenDangNhap))
            {
                taikhoan.TenDangNhap = taikhoan.TenDangNhap.Trim();
            }

            if (!string.IsNullOrWhiteSpace(taikhoan.Email))
            {
                taikhoan.Email = taikhoan.Email.Trim();
            }

            if (await _context.TaiKhoan.AnyAsync(x =>
                x.TenDangNhap == taikhoan.TenDangNhap &&
                x.MaTaiKhoan != mataikhoan))
            {
                ModelState.AddModelError(
                    nameof(taikhoan.TenDangNhap),
                    "Tên đăng nhập đã tồn tại.");
            }

            if (await _context.TaiKhoan.AnyAsync(x =>
                x.Email == taikhoan.Email &&
                x.MaTaiKhoan != mataikhoan))
            {
                ModelState.AddModelError(
                    nameof(taikhoan.Email),
                    "Email đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                TaoDanhSachPhanQuyen(taikhoan.PhanQuyen);

                return View(taikhoan);
            }

            var taiKhoanCu = await _context.TaiKhoan
                .FirstOrDefaultAsync(x => x.MaTaiKhoan == mataikhoan);

            if (taiKhoanCu == null)
                return NotFound();

            taiKhoanCu.TenDangNhap = taikhoan.TenDangNhap;
            taiKhoanCu.MatKhau = taikhoan.MatKhau;
            taiKhoanCu.Email = taikhoan.Email;
            taiKhoanCu.PhanQuyen = taikhoan.PhanQuyen;

            try
            {
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật tài khoản thành công.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Không thể cập nhật tài khoản.");

                TaoDanhSachPhanQuyen(taikhoan.PhanQuyen);

                return View(taikhoan);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? mataikhoan)
        {
            if (string.IsNullOrWhiteSpace(mataikhoan))
                return NotFound();

            var taikhoan = await _context.TaiKhoan
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaTaiKhoan == mataikhoan);

            if (taikhoan == null)
                return NotFound();

            return View(taikhoan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? mataikhoan)
        {
            if (string.IsNullOrWhiteSpace(mataikhoan))
                return NotFound();

            var taikhoan = await _context.TaiKhoan
                .FirstOrDefaultAsync(x => x.MaTaiKhoan == mataikhoan);

            if (taikhoan == null)
                return NotFound();

            try
            {
                _context.TaiKhoan.Remove(taikhoan);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa tài khoản thành công.";
            }
            catch (DbUpdateException)
            {
                TempData["DeleteError"] =
                    "Không thể xóa tài khoản vì tài khoản này đang có dữ liệu liên quan.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<string> TaoMaTaiKhoan()
        {
            var danhSachMa = await _context.TaiKhoan
                .AsNoTracking()
                .Select(x => x.MaTaiKhoan)
                .ToListAsync();

            int soLonNhat = 0;

            foreach (var ma in danhSachMa)
            {
                if (string.IsNullOrWhiteSpace(ma))
                    continue;

                if (ma.StartsWith("TK") &&
                    int.TryParse(ma.Substring(2), out int so))
                {
                    if (so > soLonNhat)
                    {
                        soLonNhat = so;
                    }
                }
            }

            return "TK" + (soLonNhat + 1).ToString("D3");
        }

        private void TaoDanhSachPhanQuyen(string? giaTri = null)
        {
            ViewBag.PhanQuyens = new List<string>
            {
                "Admin",
                "Quản lý",
                "Nhân viên"
            };

            ViewBag.PhanQuyenHienTai = giaTri;
        }
    }
}