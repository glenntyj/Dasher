using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dasher.Models;

namespace Dasher.Controllers
{
    //Authorize account controller with proper authentication scheme
    [Authorize(AuthenticationSchemes = "UserSecurity")]
    public class AccountController : Controller
    {
        private const string AUTHSCHEME = "UserSecurity";
        private const string LOGIN_SQL =
           @"SELECT * FROM users 
            WHERE username = '{0}' 
              AND password = '{1}'";

        private const string NAME_COL = "Fullname";
        private const string ROLE_COL = "Role";

        private const string REDIRECT_CNTR = "Web";
        private const string REDIRECT_ACTN = "Index";

        private const string LOGIN_VIEW = "Login";

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View(LOGIN_VIEW);
        }

        //check user account using AuthenticateUser method
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginUser user)
        {
            var username = user.Username.EscQuote();
            var password = user.Password.EscQuote();
            if (!AuthenticateUser(username, password, out ClaimsPrincipal principal))
            {
                ViewData["Message"] = "Incorrect Username or Password";
                ViewData["MsgType"] = "warning";
                return View(LOGIN_VIEW);
            }
            else
            {
                HttpContext.SignInAsync(
                   AUTHSCHEME,
                   principal,
               new AuthenticationProperties
               {
                   IsPersistent = false
               });

                if (TempData["returnUrl"] != null)
                {
                    string returnUrl = TempData["returnUrl"].ToString();
                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                }
                return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
            }
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";

                return View("Register");
            }
            else
            {
                string insert = "INSERT INTO users (Fullname, Username, Password, Role) VALUES ('{0}', '{1}', '{2}', '{3}')";
                int res = DBUtl.ExecSQL(insert, user.Fullname, user.Username, user.Password, "User");

                if (res == 1)
                {
                    TempData["Message"] = "User Added";
                    TempData["MsgType"] = "success";                  
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
            }
        }

        [Authorize]
        public IActionResult Logoff(string returnUrl = null)
        {
            HttpContext.SignOutAsync(AUTHSCHEME);
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
        }

        [AllowAnonymous]
        public IActionResult Forbidden()
        {
            return View();
        }

        //method to check user account
        private bool AuthenticateUser(string username, string password, out ClaimsPrincipal principal)
        {
            principal = null;

            DataTable ds = DBUtl.GetTable(LOGIN_SQL, username, password);
            if (ds.Rows.Count == 1)
            {
                principal =
                   new ClaimsPrincipal(
                      new ClaimsIdentity(
                         new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, ds.Rows[0]["username"].ToString()),
                        new Claim(ClaimTypes.Name, ds.Rows[0][NAME_COL].ToString()),
                        new Claim(ClaimTypes.Role, ds.Rows[0][ROLE_COL].ToString())
                         }, "Basic"
                      )
                   );
                return true;
            }
            return false;
        }

        //Show current username that login
        [HttpGet]
        public IActionResult EditUsername()
        {
            ViewData["CurrentUsername"] = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View();
        }

        //Logout user and redirect them to the login page.
        public IActionResult Relogin()
        {
            HttpContext.SignOutAsync(AUTHSCHEME);

            return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
        }
    }
}