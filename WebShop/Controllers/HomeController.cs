using Microsoft.AspNetCore.Mvc;
using WebShop.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace WebShop.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult HomePage()
        {
            using(var context = new WebShopContext())
            {
                List<Product> list = context.Products.ToList();
                ViewBag.list = list;
            }
            string id = HttpContext.Session.GetString("id");
            string role = HttpContext.Session.GetString("role");
            ViewBag.id = id;
            ViewBag.role = role;
            return View();
        }
    }
}
