using ECommerceApp.Models.ViewModels;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        // GET: /Account/Register
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);

            bool success = _userService.Register(
                               model.FullName,
                               model.Email,
                               model.Password);

            if (!success)
            {
                ModelState.AddModelError("Email",
                    "Email already registered!");
                return View(model);
            }

            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _userService.Login(model.Email, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("",
                    "Invalid email or password!");
                return View(model);
            }

            // Session mein save karo
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("Role", user.Role);

            // Admin ko admin panel, customer ko home
            if (user.Role == "Admin")
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Home");
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}