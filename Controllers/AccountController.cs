using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CustomeCookieAuthentication.DataAccess;
using CustomeCookieAuthentication.Models;
using CustomeCookieAuthentication.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CustomeCookieAuthentication.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyDbContext _context;
        public AccountController(MyDbContext context)
        {
            _context = context;

        }

        #region Login And Logout
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            //check validation


            // لیستی از کلیم ها به یوزر دادیم
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,loginViewModel.UserName ?? ""),
                new Claim("FullName","Hamed"),
                new Claim(ClaimTypes.Role,"Admin"),
                new Claim("UserId","123"),
              };


            //ست کلیم ای دنتی تی 
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = loginViewModel.RememberMe,//تیک ریممبر می
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)//طمان اکسپایر
                }
                );




            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        #endregion


        #region  Register
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {

            // if (registerViewModel.Password != registerViewModel.ConfirmPassword)
            // {
            //     ModelState.AddModelError("", " چسورد ها برابر نیستند");
            //     return View();
            // }

            if (_context.Users.Any(u => u.UserName == registerViewModel.UserName))
            {
                ModelState.AddModelError("", "یوزر نیم تکراری است");
                return View();
            }

            var newUser = new User();
            newUser.UserName = registerViewModel.UserName;
            newUser.Password = registerViewModel.Password;
            newUser.IsActive = true;
            newUser.Id = Guid.NewGuid();
            newUser.CreateDate = DateTime.Now;

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            ViewBag.IsSuccess = true;
            return View();
        }

        #endregion

    }
}