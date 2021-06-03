using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dasher.Models;

namespace Dasher.Controllers
{
    [Route("api/Dasher")]
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
                string insert = "INSERT INTO users (Fullname, Username, Password, Role, Created) VALUES ('{0}', '{1}', '{2}', '{3}', NOW())";
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

        [HttpGet("getCategory/{retailer}")]
        public IActionResult getCategory(string retailer)
        {
            string select = "SELECT DISTINCT Category FROM supermarkets WHERE Retailer='{0}'";
            List<Supermarket> model = DBUtl.GetList<Supermarket>(select, retailer);

            return Ok(model);
        }

        [HttpGet("getSubCategory/{retailer}/{category}")]
        public IActionResult getSubCategory(string retailer, string category)
        {
            string select = "SELECT DISTINCT SubCategory FROM supermarkets WHERE Retailer='{0}' AND Category='{1}'";
            List<Supermarket> model = DBUtl.GetList<Supermarket>(select, retailer, category);

            return Ok(model);
        }
    }
}
