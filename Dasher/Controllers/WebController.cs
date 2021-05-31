using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dasher.Controllers
{
    [AllowAnonymous]
    [Authorize(AuthenticationSchemes = "UserSecurity")]
    public class WebController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
