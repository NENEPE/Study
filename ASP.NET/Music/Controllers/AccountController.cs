using Microsoft.AspNetCore.Mvc;
using MusicPortal.Filters;
using MusicPortal.Models;
using MusicPortal.Repositories;
using MusicPortal.Services;
using System.Security.Cryptography;
using System.Text;

namespace MusicPortal.Controllers
{
    [Culture]
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IRepository<User> _userRepository;

        public AccountController(IAuthService authService, IRepository<User> userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        public IActionResult Login()
        {
            HttpContext.Session.SetString("path", Request.Path);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _authService.LoginCheck(model.Username, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username, password, or account not activated");
                return View(model);
            }
            else if (!user.IsActive)
            {
                ModelState.AddModelError("", "Account is not activated. Please wait for admin activation");
                return View(model);
            }

            _authService.UserSet(user);

            _authService.CookieSet("login", user.Username);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            HttpContext.Session.SetString("path", Request.Path);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _authService.UsernameCheck(model.Username))
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View(model);
            }

            if (await _authService.EmailCheck(model.Email))
            {
                ModelState.AddModelError("Email", "Email already registered");
                return View(model);
            }

            _authService.HashPassword(model.Password, out string salt, out string hashedPassword);

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = hashedPassword,
                IsAdmin = false,
                Salt = salt
            };

            var result = await _authService.RegisterCheck(user);
            if (result)
            {
                _authService.CookieSet("login", user.Username);
                TempData["SuccessMessage"] = "Registration successful! Please wait for administrator approval.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed. Please try again.");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("login");
            return RedirectToAction("Index", "Home");
        }
    }
}