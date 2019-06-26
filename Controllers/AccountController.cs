using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Infrastructure;
using OnlineStore.Models;

namespace OnlineStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConnection _connection;

        public AccountController(IConnection connection)
        {
            _connection = connection;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignIn(string returnUrl = null)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToReturnUrl(returnUrl);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInModel model, string returnUrl = null, CancellationToken cancellationToken = default)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToReturnUrl(returnUrl);
            }

            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            const string sql = @"Select * from users where username = @username and password = @password";
            var user = await _connection.QueryAsync<SignInModel>(sql, new { username = model.UserName, password = model.Password });
            if (user.Count() <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, model.UserName)
                };
            var identity = new ClaimsIdentity(claims);
            await HttpContext.SignInAsync(new ClaimsPrincipal(identity));
            return RedirectToReturnUrl(returnUrl);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(SignInModel model, string returnUrl = null)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToReturnUrl(returnUrl);
            }

            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            const string sql = @"Select * from users where username = @username and password = @password";
            var user = await _connection.QueryAsync<SignInModel>(sql, new { username = model.UserName, password = model.Password });

            if (user.Count() <= 0)
            {
                const string sqlBuff = @"Insert into users (username, password, email) values (@username, @password, @email)";
                _connection.Execute(sqlBuff, new { username = model.UserName, password = model.Password, email = model.Email });
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, model.UserName)
                };
                var identity = new ClaimsIdentity(claims);
                await HttpContext.SignInAsync(new ClaimsPrincipal(identity));

                return RedirectToReturnUrl(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "User Exists.");

            return View(model);
        }

        [HttpGet]
        [Route("signout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return LocalRedirect(Url.Content("~/"));
        }

        private IActionResult RedirectToReturnUrl(string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            return LocalRedirect(returnUrl);
        }
    }
}