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

        protected string CurrentUsername => HttpContext.Session.GetString("Username") ?? string.Empty;
        protected string CurrentRole => HttpContext.Session.GetString("Role") ?? string.Empty;

        protected bool IsAdmin => CurrentRole == "Admin";
        protected bool IsStaff => CurrentRole == "Staff";
    }
}
