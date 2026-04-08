using Microsoft.AspNetCore.Mvc;

namespace ItemManager.Web.Controllers
{
    public class DashboardController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
