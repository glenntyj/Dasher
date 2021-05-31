using Dasher.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dasher.Controllers
{
    [Authorize(AuthenticationSchemes = "UserSecurity")]
    public class UserController : Controller
    {
        private List<User> users = DBUtl.GetList<User>(@"SELECT * FROM users");

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Details(string username)
        {
            return PartialView("_UserDetails", users.Where(w => w.Username == username).FirstOrDefault());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult UserTable()
        {
            return PartialView("_UserTable", users);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            string delete = "DELETE FROM users WHERE Username='{0}'";
            int res = DBUtl.ExecSQL(delete, id);

            if (res == 1)
            {
                TempData["Msg"] = "User deleted!";
                TempData["MsgType"] = "success";
            }
            else
            {
                TempData["Msg"] = DBUtl.DB_Message;
                TempData["MsgType"] = "danger";
            }

            return RedirectToAction("Index");
        }
    }
}
