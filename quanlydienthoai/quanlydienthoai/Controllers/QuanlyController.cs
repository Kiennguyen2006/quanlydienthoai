using Microsoft.AspNetCore.Mvc;

namespace quanlydienthoai.Controllers
{
    public class QuanlyController : Controller
    {
        public IActionResult Quanly()
        {
            ViewBag.quyen = TempData["phanquyen"]?.ToString() ?? "";
            ViewBag.tentk = TempData["tentk"]?.ToString() ?? "";
            return View();
        }
    }
}