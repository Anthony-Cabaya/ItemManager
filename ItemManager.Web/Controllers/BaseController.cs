using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ItemManager.Web.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                context.Result = RedirectToAction("Login", "Login");
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
