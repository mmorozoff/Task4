using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task4.ViewModels; // пространство имен моделей RegisterModel и LoginModel
using Task4.Models; // пространство имен UserContext и класса User
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Task4.Controllers
{
    public class AccountController : Controller
    {
        private UserContext db;
        public AccountController(UserContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password && u.State == "Unblocked");
                if (user != null)
                {
                    user.LoginTime = DateTime.Now;
                    db.Users.Update(user);
                    await db.SaveChangesAsync();
                    await Authenticate(model.Email); // аутентификация
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    db.Users.Add(new User { Email = model.Email, Password = model.Password, Name = model.Name, RegistrationTime = DateTime.Now, LoginTime = DateTime.Now, State = "Unblocked"});
                    await db.SaveChangesAsync();

                    await Authenticate(model.Email); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Action(List<string> AreChecked, IFormCollection form)
        {
            int flag = 0;
            foreach (var item in AreChecked)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == item);
                if (form.Keys.Contains("delete"))
                {
                    if (user.Email == User.Identity.Name)
                        flag = 1;
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
                if (form.Keys.Contains("block"))
                {
                    user.State = "Blocked";
                    if (user.Email == User.Identity.Name)
                        flag = 1;
                    db.Users.Update(user);
                    db.SaveChanges();
                }
                if (form.Keys.Contains("unblock"))
                {
                    user.State = "UnBlocked";
                    db.Users.Update(user);
                    db.SaveChanges();
                }
            }
            if (flag == 1)
                return RedirectToAction("Login", "Account");
            else
                return RedirectToAction("Index", "Home");
        }
    }
}
