using Dasher.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Dasher.Controllers
{
    [Authorize(AuthenticationSchemes = "UserSecurity")]
    public class NoteController : Controller
    {
        private List<Note> notes = DBUtl.GetList<Note>(@"SELECT * FROM notes");

        public IActionResult Index()
        {
            ViewData["CurrentUsername"] = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View(notes);
        }
    }
}
