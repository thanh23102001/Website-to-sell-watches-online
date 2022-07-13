using Microsoft.AspNetCore.Mvc;
using WebShop.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

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
            string sum = HttpContext.Session.GetString("slsp");
            if (sum == null)
            {
                sum = "0";
            }
            ViewBag.id = id;
            ViewBag.role = role;
            ViewBag.slsp = sum;
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

        [HttpPost]
        public IActionResult Register(int type)
        {
            Account account = new Account();
            string message = string.Empty;
            if (type == 1)
            {
                message = "Sign Up Success!";
            }
            using (var context = new WebShopContext())
            {
                account.Role = 2;
                account.Username = HttpContext.Request.Form["user"];
                account.Password = HttpContext.Request.Form["pass"];
                account.Email = HttpContext.Request.Form["email"];
                context.Accounts.Add(account);
                context.SaveChanges();
            }
            SetAlert(message, type);
            return Redirect("/Home/FormLogin");
        }

        protected void SetAlert(string message, int type)
        {
            TempData["AlertMessage"] = message;
            if (type == 1)
            {
                TempData["AlertType"] = "alert-success";
            }
            else if (type == 2)
            {
                TempData["AlertType"] = "alert-warning";
            }
            else if (type == 3)
            {
                TempData["AlertType"] = "alert-danger";
            }
            else
            {
                TempData["AlertType"] = "alert-info";
            }
        }

      
        public IActionResult AddToCart(int id)
        {
            //sp1 co sl1
            //sp2 co sl2
            IDictionary<int, int> cart = new Dictionary<int, int>();
            cart.Add(id, 1);
            int sum = 0;
            foreach (KeyValuePair<int, int> item in cart)
            {
                sum += item.Value;
            }
            HttpContext.Session.SetString("slsp", sum.ToString());
            //chuyen cart ve string luu trong session
            string jsonData = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("cart", jsonData);
            return Redirect("/Home/HomePage");
        }
    }
}

