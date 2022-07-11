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
            using (var context = new WebShopContext())
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

        public IActionResult FormLogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login()
        {
            string user = HttpContext.Request.Form["user"];
            string pass = HttpContext.Request.Form["pass"];
            using (var context = new WebShopContext())
            {
                Account a = context.Accounts.Where(x => x.Password == pass && x.Username == user).SingleOrDefault();
                if (a != null)
                {
                    HttpContext.Session.SetString("id", a.Id.ToString());
                    HttpContext.Session.SetString("role", a.Role.ToString());
                    return Redirect("/Home/HomePage");
                }
                else
                {
                    return Redirect("/Home/FormLogin");
                }
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/Home/FormLogin");

        }

        public IActionResult Register()
        {

            return View();
        }
    }
}
