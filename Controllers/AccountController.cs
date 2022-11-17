using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CustomeCookieAuthentication.Common;
using CustomeCookieAuthentication.DataAccess;
using CustomeCookieAuthentication.Models;
using CustomeCookieAuthentication.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomeCookieAuthentication.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyDbContext _context;
        private readonly EncryptionUtility _encryptionUtility;
        public AccountController(MyDbContext context, EncryptionUtility encryptionUtility)
        {
            _context = context;
            _encryptionUtility = encryptionUtility;
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
            var findedUser = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginViewModel.UserName);


            if (findedUser == null)
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View();
            }
            var hashPassword = _encryptionUtility.HashSHA256(loginViewModel.Password);
            if (findedUser.Password != hashPassword)
            {
                ModelState.AddModelError("", "invalid username or password");
                return View();
            }

            if (!findedUser.IsActive)
            {
                ModelState.AddModelError("", "User Is Not Active");
                return View();
            }


            // لیستی از کلیم ها به یوزر دادیم
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,findedUser.UserName),
                new Claim("FullName",findedUser.UserName),
                new Claim(ClaimTypes.Role,"Admin"),
                new Claim("UserId",findedUser.Id.ToString()),
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
            newUser.Password = _encryptionUtility.HashSHA256(registerViewModel.Password);
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