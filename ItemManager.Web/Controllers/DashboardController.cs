using Microsoft.AspNetCore.Mvc;

namespace ItemManager.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
