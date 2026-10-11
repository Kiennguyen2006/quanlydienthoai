using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using quanlydienthoai.Models;

namespace quanlydienthoai.Controllers
{
    public class KiemtrathongtinController : Controller
    {
        private readonly QuanLyBanDienThoaiContext _context;

        public KiemtrathongtinController(
            QuanLyBanDienThoaiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Dangnhap()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Dangnhap(
            string MatKhau,
            string TenDangNhap)
        {
            if (string.IsNullOrWhiteSpace(TenDangNhap) ||
                string.IsNullOrWhiteSpace(MatKhau))
            {
                ViewBag.Thongbao =
                    "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!";

                return View();
            }

            var taiKhoan = _context.TaiKhoan
                .FirstOrDefault(x =>
                    x.TenDangNhap == TenDangNhap &&
                    x.MatKhau == MatKhau);

            if (taiKhoan != null)
            {
                TempData["tentk"] = taiKhoan.TenDangNhap;
                TempData["phanquyen"] = taiKhoan.PhanQuyen;

                return RedirectToAction(
                    "Quanly",
                    "Quanly");
            }

            ViewBag.Thongbao =
                "Tên đăng nhập hoặc mật khẩu không đúng!";

            return View();
        }

        public IActionResult Dangxuat()
        {
            return RedirectToAction(
                "Dangnhap",
                "Kiemtrathongtin");
        }

        public async Task<IActionResult> Quenmk(string email)
        {
            if (email == null)
            {
                return View();
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Thongbao =
                    "Vui lòng nhập email!";

                return View();
            }

            email = email.Trim();

            var taiKhoan = _context.TaiKhoan
                .FirstOrDefault(x => x.Email == email);

            if (taiKhoan == null)
            {
                ViewBag.Thongbao =
                    "Email không tồn tại!";

                return View();
            }

            Random rd = new Random();

            string ma = rd
                .Next(100000, 1000000)
                .ToString();

            TempData["MaOTP"] = ma;
            TempData["Email"] = email;

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    "Hệ thống quản lý",
                    "kiennguyenfa12345678@gmail.com"
                )
            );

            message.To.Add(
                new MailboxAddress(
                    "",
                    email
                )
            );

            message.Subject =
                "Mã khôi phục mật khẩu";

            message.Body = new TextPart("plain")
            {
                Text =
         "🔐 KHÔI PHỤC MẬT KHẨU\n\n" +
         "Xin chào,\n\n" +
         "Chúng tôi đã nhận được yêu cầu khôi phục mật khẩu cho tài khoản của bạn.\n\n" +
         "MÃ XÁC NHẬN: " + ma + "\n\n" +
         "Vui lòng nhập mã này vào hệ thống để tiếp tục đặt lại mật khẩu.\n\n" +
         "⚠️ Lưu ý:\n" +
         "- Mã xác nhận chỉ được sử dụng một lần.\n" +
         "- Không chia sẻ mã này với bất kỳ ai.\n" +
         "- Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.\n\n" +
         "Trân trọng,\n" +
         "Đội ngũ KMNCA\n" +
         "Hệ thống quản lý cửa hàng điện thoại"
            };

            try
            {
                using (var smtp = new SmtpClient())
                {
                    await smtp.ConnectAsync(
                        "smtp.gmail.com",
                        587,
                        SecureSocketOptions.StartTls
                    );

                    await smtp.AuthenticateAsync(
                        "kiennguyenfa12345678@gmail.com",
                        "xokepjphvempgazk"
                    );

                    await smtp.SendAsync(message);

                    await smtp.DisconnectAsync(true);
                }

                return RedirectToAction(
                    "Nhapma",
                    "Kiemtrathongtin");
            }
            catch (Exception)
            {
                TempData.Remove("MaOTP");
                TempData.Remove("Email");

                ViewBag.Thongbao =
                    "Không thể gửi email. Vui lòng kiểm tra cấu hình Gmail.";

                return View();
            }
        }
        public IActionResult Nhapma(string maotp)
        {
            if (maotp == null)
            {
                return View();
            }
            string otp = TempData.Peek("MaOTP")?.ToString();
            if (maotp == otp)
            {
                TempData["email"] = TempData["Email"];
                return RedirectToAction("Nhaplaimk", "Kiemtrathongtin");
            }
            else
            {
                ViewBag.Thongbao =
                    "Mã OTP không đúng!";
                return View("Nhapma", "Kiemtrathongtin");
            }
        }
        public IActionResult Nhaplaimk(string matkhau, string nhaplaimk)
        {
            if (matkhau == null && nhaplaimk == null)
            {
                return View();
            }
            if (matkhau == nhaplaimk)
            {
                var bang = _context.TaiKhoan.ToList();
                for (int i = 0; i < bang.Count; i++)
                {
                    string email = TempData["email"].ToString();
                    if (bang[i].Email == email)
                    {
                        bang[i].MatKhau = matkhau;
                        _context.SaveChanges();
                        return RedirectToAction("Dangnhap", "Kiemtrathongtin");
                    }
                }
            }
            return View();
        }
        public IActionResult Taotaikhoan()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Taotk(TaiKhoan taikhoan)
        {
            string maLonNhat = _context.TaiKhoan
            .OrderByDescending(x => x.MaTaiKhoan)
            .Select(x => x.MaTaiKhoan)
            .FirstOrDefault();

            int so = 1;

            if (!string.IsNullOrEmpty(maLonNhat))
            {
                string phanSo = maLonNhat.Substring(2);
                so = int.Parse(phanSo) + 1;
            }

            string maTaiKhoan = "TK" + so.ToString("D3");
            ModelState.Remove("MaTaiKhoan");
            ModelState.Remove("PhanQuyen");
            ModelState.Remove("NgayTao");
            taikhoan.MaTaiKhoan = maTaiKhoan;
            taikhoan.PhanQuyen = "Nhân viên";
            taikhoan.NgayTao = DateTime.Now;
            if (ModelState.IsValid)
            {
                int l = 0;
                var bang = _context.TaiKhoan.ToList();
                foreach (var item in bang)
                {
                    if (item.Email == taikhoan.Email)
                    {
                        l = 1;
                        break;
                    }
                    else
                    {
                        l = 0;
                    }
                }
                if (l == 1)
                {
                    ViewBag.Thongbao = "Email đã tồn tại vui lòng nhập mới!!!";
                    return View("Taotaikhoan", "Kiemtrathongtin");
                }
                else
                {
                    _context.TaiKhoan.Add(taikhoan);
                    _context.SaveChanges();
                    return RedirectToAction("Dangnhap", "Kiemtrathongtin");
                }
            }
            else
            {
                ViewBag.Thongbao = "Vui lòng nhập đầy đủ thông tin!";
                return View("Taotaikhoan", "Kiemtrathongtin");
            }
            return View();
        }
    }
}