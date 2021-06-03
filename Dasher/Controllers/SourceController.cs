using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dasher.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using HtmlAgilityPack;

namespace Dasher.Controllers
{
    [Authorize(AuthenticationSchemes = "UserSecurity")]
    public class SourceController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Supermarket()
        {
            List<Supermarket> supermarket = DBUtl.GetList<Supermarket>("SELECT DISTINCT Retailer FROM supermarkets");
            ViewData["retailers"] = new SelectList(supermarket, "Retailer", "Retailer");

            return View();
        }

        [Authorize]
        public IActionResult GetSource(string id)
        {
            string category = id.Split("*")[0].Replace("-", " ");
            string subCategory = id.Split("*")[1].Replace("-", " ");

            string select = @"SELECT Url FROM supermarkets WHERE Category='{0}' AND SubCategory='{1}'";

            List<Supermarket> supermarket = DBUtl.GetList<Supermarket>(select, category, subCategory);
            string url = supermarket[0].Url;

            HtmlWeb web = new HtmlWeb();
            var htmlDocument = web.Load(url);
            var products = htmlDocument.DocumentNode.SelectNodes("//*");

            string str = "";

            if (products != null)
            {
                foreach (var product in products)
                {
                    str += SetUCase(product.InnerText.Replace("&amp;", "&"));
                }
            }

            ViewData["Results"] = str;

            return PartialView("_ResultTable");
        }

        public static string SetUCase(string unformattedString)
        {
            string formattedString = "";

            foreach (var s in unformattedString.Split(" "))
            {
                if (s.Length == 0)
                {
                    formattedString += s;
                }
                else if (s.Length == 1)
                {
                    formattedString += char.ToUpper(s[0]);
                }
                else
                {
                    formattedString += char.ToUpper(s[0]) + s.Substring(1);
                }
                formattedString += " ";
            }
            return formattedString.Trim();
        }
    }
}
