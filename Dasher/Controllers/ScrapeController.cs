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
using System.Dynamic;

namespace Dasher.Controllers
{
    [Authorize(AuthenticationSchemes = "UserSecurity")]
    public class ScrapeController : Controller
    {
        private static dynamic getModel()
        {
            List<Website> websites = DBUtl.GetList<Website>("SELECT * FROM websites");
            List<Supermarket> supermarkets = DBUtl.GetList<Supermarket>("SELECT * FROM supermarkets");

            dynamic model = new ExpandoObject();
            model.Website = websites;
            model.Supermarket = supermarkets;

            return model;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(getModel()); 
        }

        [Authorize]
        public IActionResult Source(int id)
        {
            List<Website> websites = DBUtl.GetList<Website>("SELECT * FROM websites WHERE Id={0}", id);
            var url = websites[0].Url;
            var company = websites[0].Company;

            string delete = "DELETE FROM supermarkets WHERE Retailer='{0}'";
            DBUtl.ExecSQL(delete, company);

            HtmlWeb web = new HtmlWeb();
            var htmlDocument = web.Load(url);
            var categories = htmlDocument.DocumentNode.SelectNodes("//ul[@data-testid='categoryContainer']//p");
            var categoriesURL = htmlDocument.DocumentNode.SelectNodes("//ul[@data-testid='categoryContainer']//span/@href");
            var subCategoriesURL = htmlDocument.DocumentNode.SelectNodes("//ul[@data-testid='subCategory']//a/@href");

            List<string> lstCategory = new List<string>();
            List<string> lstCategoryURL = new List<string>();
            List<string> lstSubCategoryURL = new List<string>();

            string retailer = "NTUC FairPrice";
            string str = "";

            if (categories != null)
            {
                foreach (var category in categories)
                {
                    lstCategory.Add(SetUCase(category.InnerText.Replace("&amp;", "&")));
                }
            }

            if (categoriesURL != null)
            {
                foreach (var categoryURL in categoriesURL)
                {
                    lstCategoryURL.Add("https://www.fairprice.com.sg" + categoryURL.Attributes["href"].Value);
                }
            }

            if (subCategoriesURL != null)
            {
                foreach (var subCategoryURL in subCategoriesURL)
                {
                    lstSubCategoryURL.Add("https://www.fairprice.com.sg" + subCategoryURL.Attributes["href"].Value);
                }
            }

            int countSub = 0;          
            for (int count = 0; count < lstCategory.Count(); count++)
            {
                var subCategories = htmlDocument.DocumentNode.SelectNodes(string.Format("//li[@order='{0}']//a", count));
                if (subCategories != null)
                {
                    bool first = true;
                    foreach (var subCategory in subCategories)
                    {
                        str = SetUCase(subCategory.InnerText.Replace("&amp;", "&"));                     
                        string insert = @"INSERT INTO supermarkets (Retailer, Category, SubCategory, Url)
                                    VALUE ('{0}', '{1}', '{2}', '{3}')";

                        if (count == 0 && first)
                        {
                            DBUtl.ExecSQL(insert, retailer, lstCategory[count], "View All", lstCategoryURL[count]);
                            DBUtl.ExecSQL(insert, retailer, lstCategory[count], str, lstSubCategoryURL[countSub]);
                            first = false;
                            countSub++;                          
                        }
                        else if (first)
                        {
                            DBUtl.ExecSQL(insert, retailer, lstCategory[count], str, lstCategoryURL[count]);
                            first = false;
                        }
                        else
                        {
                            if (str != "Infant Formula")
                            {
                                DBUtl.ExecSQL(insert, retailer, lstCategory[count], str, lstSubCategoryURL[countSub]);
                                countSub++;
                            }
                        }
                    }
                }
            }

            string update = "UPDATE websites SET LastScraped = NOW() WHERE Id={0}";
            DBUtl.ExecSQL(update, id);

            return View("Index", getModel());
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
