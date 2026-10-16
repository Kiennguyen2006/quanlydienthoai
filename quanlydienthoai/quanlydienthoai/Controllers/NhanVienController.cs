using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlydienthoai.Models;

namespace quanlydienthoai.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly QuanLyBanDienThoaiContext _context;
        private readonly IWebHostEnvironment _environment;

        public NhanVienController(
            QuanLyBanDienThoaiContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string? timkiem)
        {
            var danhSach = _context.NhanVien
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(timkiem))
            {
                timkiem = timkiem.Trim();

                danhSach = danhSach.Where(x =>
                    x.MaNhanVien.Contains(timkiem) ||
                    x.HoTen.Contains(timkiem) ||
                    (x.Email != null && x.Email.Contains(timkiem)) ||
                    (x.ChucVu != null && x.ChucVu.Contains(timkiem)));
            }

            ViewBag.TimKiem = timkiem;

            return View(await danhSach
                .OrderBy(x => x.MaNhanVien)
                .ToListAsync());
        }

        public async Task<IActionResult> Details(string? manhanvien)
        {
            if (string.IsNullOrWhiteSpace(manhanvien))
                return NotFound();

            var nhanvien = await _context.NhanVien
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaNhanVien == manhanvien);

            if (nhanvien == null)
                return NotFound();

            return View(nhanvien);
        }

        [HttpGet]
        public IActionResult Create()
        {
            TaoDanhSachChucVu();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            NhanVien nhanvien,
            IFormFile? fileAnh)
        {
            ModelState.Remove(nameof(nhanvien.MaNhanVien));
            ModelState.Remove(nameof(nhanvien.Anh));

            if (!KiemTraChucVu(nhanvien.ChucVu))
            {
                ModelState.AddModelError(
                    nameof(nhanvien.ChucVu),
                    "Vui lòng chọn chức vụ hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                TaoDanhSachChucVu(nhanvien.ChucVu);
                return View(nhanvien);
            }

            nhanvien.MaNhanVien = TaoMaNhanVien();

            nhanvien.Anh = await LuuAnh(fileAnh);

            try
            {
                _context.NhanVien.Add(nhanvien);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!string.IsNullOrWhiteSpace(nhanvien.Anh))
                    XoaAnh(nhanvien.Anh);

                ModelState.AddModelError(
                    string.Empty,
                    "Không thể thêm nhân viên. Vui lòng kiểm tra lại dữ liệu.");

                TaoDanhSachChucVu(nhanvien.ChucVu);

                return View(nhanvien);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? manhanvien)
        {
            if (string.IsNullOrWhiteSpace(manhanvien))
                return NotFound();

            var nhanvien = await _context.NhanVien
                .FirstOrDefaultAsync(x => x.MaNhanVien == manhanvien);

            if (nhanvien == null)
                return NotFound();

            TaoDanhSachChucVu(nhanvien.ChucVu);

            return View(nhanvien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string? manhanvien,
            NhanVien nhanvien,
            IFormFile? fileAnh)
        {
            if (string.IsNullOrWhiteSpace(manhanvien))
                return NotFound();

            if (manhanvien != nhanvien.MaNhanVien)
                return NotFound();

            ModelState.Remove(nameof(nhanvien.MaNhanVien));
            ModelState.Remove(nameof(nhanvien.Anh));

            if (!KiemTraChucVu(nhanvien.ChucVu))
            {
                ModelState.AddModelError(
                    nameof(nhanvien.ChucVu),
                    "Vui lòng chọn chức vụ hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                TaoDanhSachChucVu(nhanvien.ChucVu);
                return View(nhanvien);
            }

            var nhanVienCu = await _context.NhanVien
                .FirstOrDefaultAsync(x => x.MaNhanVien == manhanvien);

            if (nhanVienCu == null)
                return NotFound();

            string? anhCu = nhanVienCu.Anh;
            string? anhMoi = null;

            nhanVienCu.HoTen = nhanvien.HoTen;
            nhanVienCu.NgaySinh = nhanvien.NgaySinh;
            nhanVienCu.GioiTinh = nhanvien.GioiTinh;
            nhanVienCu.Email = nhanvien.Email;
            nhanVienCu.DiaChi = nhanvien.DiaChi;
            nhanVienCu.ChucVu = nhanvien.ChucVu;
            nhanVienCu.NgayVaoLam = nhanvien.NgayVaoLam;
            nhanVienCu.MoTa = nhanvien.MoTa;

            if (fileAnh != null && fileAnh.Length > 0)
            {
                anhMoi = await LuuAnh(fileAnh);
                nhanVienCu.Anh = anhMoi;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!string.IsNullOrWhiteSpace(anhMoi))
                    XoaAnh(anhMoi);

                ModelState.AddModelError(
                    string.Empty,
                    "Không thể cập nhật nhân viên. Vui lòng kiểm tra lại dữ liệu.");

                TaoDanhSachChucVu(nhanvien.ChucVu);

                return View(nhanvien);
            }

            if (!string.IsNullOrWhiteSpace(anhMoi))
                XoaAnh(anhCu);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? manhanvien)
        {
            if (string.IsNullOrWhiteSpace(manhanvien))
                return NotFound();

            var nhanvien = await _context.NhanVien
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaNhanVien == manhanvien);

            if (nhanvien == null)
                return NotFound();

            return View(nhanvien);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? manhanvien)
        {
            if (string.IsNullOrWhiteSpace(manhanvien))
                return NotFound();

            var nhanvien = await _context.NhanVien
                .FirstOrDefaultAsync(x => x.MaNhanVien == manhanvien);

            if (nhanvien == null)
                return NotFound();

            try
            {
                _context.NhanVien.Remove(nhanvien);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa nhân viên thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["DeleteError"] =
                    "Không thể xóa nhân viên vì nhân viên này đang có hóa đơn hoặc dữ liệu liên quan.";
                return RedirectToAction(nameof(Delete), new { manhanvien });
            }
        }
        private string TaoMaNhanVien()
        {
            var danhSach = _context.NhanVien
                .AsNoTracking()
                .Select(x => x.MaNhanVien)
                .ToList();

            int soLonNhat = 0;

            foreach (var ma in danhSach)
            {
                if (string.IsNullOrWhiteSpace(ma))
                    continue;

                if (ma.StartsWith("NV") &&
                    int.TryParse(ma.Substring(2), out int so))
                {
                    if (so > soLonNhat)
                        soLonNhat = so;
                }
            }

            return "NV" + (soLonNhat + 1).ToString("D3");
        }

        private void TaoDanhSachChucVu(string? giaTri = null)
        {
            ViewBag.ChucVus = new List<string>
            {
                "Quản lý",
                "Nhân viên",
                "Kỹ thuật viên"
            };

            ViewBag.ChucVuHienTai = giaTri;
        }

        private bool KiemTraChucVu(string? chucVu)
        {
            return chucVu == "Quản lý"
                || chucVu == "Nhân viên"
                || chucVu == "Kỹ thuật viên";
        }

        private async Task<string?> LuuAnh(IFormFile? fileAnh)
        {
            if (fileAnh == null || fileAnh.Length == 0)
                return null;

            string[] duoiChoPhep =
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            string duoi = Path.GetExtension(fileAnh.FileName)
                .ToLowerInvariant();

            if (!duoiChoPhep.Contains(duoi))
                return null;

            string thuMuc = Path.Combine(
                _environment.WebRootPath,
                "images");

            if (!Directory.Exists(thuMuc))
                Directory.CreateDirectory(thuMuc);

            string tenFile =
                Guid.NewGuid().ToString("N") + duoi;

            string duongDan =
                Path.Combine(thuMuc, tenFile);

            using (var stream = new FileStream(
                duongDan,
                FileMode.Create))
            {
                await fileAnh.CopyToAsync(stream);
            }

            return "/images/" + tenFile;
        }

        private void XoaAnh(string? duongDanAnh)
        {
            if (string.IsNullOrWhiteSpace(duongDanAnh))
                return;

            string tenFile =
                Path.GetFileName(duongDanAnh);

            string duongDan =
                Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    tenFile);

            if (System.IO.File.Exists(duongDan))
                System.IO.File.Delete(duongDan);
        }
    }
}