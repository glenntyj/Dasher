using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dasher.Models;

namespace Dasher.Controllers
{
    [Route("api/User")]
    public class ApiController : Controller
    {
        private List<User> users = DBUtl.GetList<User>(@"SELECT * FROM users");

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("AddUser/{fullname}/{username}/{password}/{role}")]
        public IActionResult AddUser(string fullname, string username, string password, string role)
        {
            var model = users.Where(w => w.Username.ToLower() == username.ToLower()).FirstOrDefault();

            if (model == null)
            {
                string insert = "INSERT INTO users (Fullname, Username, Password, Role) VALUES ('{0}', '{1}', '{2}', '{3}')";
                int res = DBUtl.ExecSQL(insert, fullname, username, password, role);

                if (res == 1)
                {
                    return Ok(1);
                }
                else
                {
                    return Ok(0);
                }
            }
            else
            {
                return Ok(-1);
            }
        }
    }
}
