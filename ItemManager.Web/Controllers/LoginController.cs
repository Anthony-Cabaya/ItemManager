using ItemManager.Core.Interfaces;
using ItemManager.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ItemManager.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserRepository _userRepository;

        public LoginController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                // Check lockout first
                if (IsLockedOut())
                {
                    var lockoutExpiry = DateTime.Parse(
                        HttpContext.Session.GetString("LockoutTime")!);
                    var remaining = Math.Max(0, (int)(lockoutExpiry - DateTime.Now).TotalSeconds);
                    ViewData["LockoutSeconds"] = remaining;
                    ModelState.AddModelError(string.Empty,
                        $"Too many failed attempts. Try again in {remaining} seconds.");
                    return View(model);
                }

                if (!ModelState.IsValid)
                    return View(model);

                var user = await _userRepository.ValidateUserAsync(model.Username, model.Password);

                if (user == null)
                {
                    var attempts = RegisterFailedAttempt();
                    var remaining = 5 - attempts;

                    if (remaining <= 0)
                    {
                        var lockoutExpiry = DateTime.Parse(
                            HttpContext.Session.GetString("LockoutTime")!);

                        var lockoutRemaining = Math.Max(0,
                            (int)(lockoutExpiry - DateTime.Now).TotalSeconds);

                        ViewData["LockoutSeconds"] = lockoutRemaining;
                        ModelState.AddModelError(string.Empty,
                            $"Too many failed attempts. Try again in {lockoutRemaining} seconds.");
                    } 
                    else
                    {
                        ModelState.AddModelError(string.Empty,
                            $"Invalid username or password. {remaining} attempt(s) remaining.");
                    }
                    return View(model);
                }

                // Success - clear lockout
                HttpContext.Session.Remove("LoginAttempts");
                HttpContext.Session.Remove("LockoutTime");

                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
        }

        private bool IsLockedOut()
        {
            var lockoutTime = HttpContext.Session.GetString("LockoutTime");
            if (lockoutTime == null) return false;

            var lockoutExpiry = DateTime.Parse(lockoutTime);
            if (DateTime.Now < lockoutExpiry) return true;

            HttpContext.Session.Remove("LockoutTime");
            HttpContext.Session.Remove("LoginAttempts");
            return false;
        }

        private int RegisterFailedAttempt()
        {
            var attempts = HttpContext.Session.GetInt32("LoginAttempts") ?? 0;
            attempts++;
            HttpContext.Session.SetInt32("LoginAttempts", attempts);

            if (attempts >= 5)
            {
                HttpContext.Session.SetString("LockoutTime",
                    DateTime.Now.AddMinutes(5).ToString());
            }
            return attempts;
        }

    }
}
