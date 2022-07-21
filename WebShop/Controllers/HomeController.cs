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
                List<Product> list = context.Products.Where(x => x.Status == true).ToList();
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
                account.Fullname = HttpContext.Request.Form["name"];
                account.Username = HttpContext.Request.Form["user"];
                account.Password = HttpContext.Request.Form["pass"];
                account.Email = HttpContext.Request.Form["email"];
                account.Phone = HttpContext.Request.Form["phone"];
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
            string cart1 = HttpContext.Session.GetString("cart");
            IDictionary<int, int> cart = new Dictionary<int, int>();
            if (cart1 != null)
            {
                cart = JsonSerializer.Deserialize<IDictionary<int, int>>(cart1);
                if (cart.ContainsKey(id))
                {
                    cart[id] += 1;
                }
                else
                {
                    cart.Add(id, 1);
                }
            }
            else
            {
                cart.Add(id, 1);
            }
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

        public IActionResult ViewCart()
        {
            string json = HttpContext.Session.GetString("cart");
            IDictionary<int, int> cart; //= JsonSerializer.Deserialize<IDictionary<int, int>>(json);
            int? sum = 0;
            if (json == null)
            {
                ViewBag.number = null;
            }
            else
            {
                cart = JsonSerializer.Deserialize<IDictionary<int, int>>(json);
                foreach (KeyValuePair<int, int> item in cart)
                {
                    using (var context = new WebShopContext())
                    {
                        Product product = context.Products.Where(x => x.Id == item.Key).SingleOrDefault();
                        sum += product.Price * item.Value;
                    }
                }
                ViewBag.sum = sum;
                ViewBag.cart = cart;
                ViewBag.number = 1;
            }
            return View();
        }

        public IActionResult AddSP(int id)
        {
            string cart1 = HttpContext.Session.GetString("cart");
            IDictionary<int, int> cart = new Dictionary<int, int>();
            if (cart1 != null)
            {
                cart = JsonSerializer.Deserialize<IDictionary<int, int>>(cart1);
                if (cart.ContainsKey(id))
                {
                    cart[id] += 1;
                }
            }
            int sum = 0;
            foreach (KeyValuePair<int, int> item in cart)
            {
                sum += item.Value;
            }
            HttpContext.Session.SetString("slsp", sum.ToString());
            //chuyen cart ve string luu trong session
            string jsonData = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("cart", jsonData);
            return Redirect("/Home/ViewCart");
        }
        public IActionResult XoaSP(int id)
        {
            string cart1 = HttpContext.Session.GetString("cart");
            IDictionary<int, int> cart = new Dictionary<int, int>();
            if (cart1 != null)
            {
                cart = JsonSerializer.Deserialize<IDictionary<int, int>>(cart1);
                if (cart.ContainsKey(id))
                {
                    if (cart[id] == 1)
                    {
                        cart.Remove(id);
                    }
                    else
                    {
                        cart[id] -= 1;
                    }
                }
            }
            int sum = 0;
            foreach (KeyValuePair<int, int> item in cart)
            {
                sum += item.Value;
            }
            HttpContext.Session.SetString("slsp", sum.ToString());
            //chuyen cart ve string luu trong session
            string jsonData = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("cart", jsonData);
            return Redirect("/Home/ViewCart");
        }
        public IActionResult Order(string sum)
        {
            ViewBag.sum = sum;
            return View();
        }



        public IActionResult ThanhToan(int sum)
        {
            string note = HttpContext.Request.Form["note"];
            string address = HttpContext.Request.Form["address"];
            string phone = HttpContext.Request.Form["phone"];
            string name = HttpContext.Request.Form["name"];
            string id = HttpContext.Session.GetString("id");
            using var context = new WebShopContext();
            List<Order> list = context.Orders.ToList();
            int count = list.Count + 1;
            Order order = new Order();
            order.Purchases = count;
            order.Customer = int.Parse(id);
            order.TotalPrice = sum;
            order.Note = note;
            order.Address = address;
            order.Phone = phone;
            order.Name = name;
            context.Orders.Add(order);
            context.SaveChanges();
            string cart1 = HttpContext.Session.GetString("cart");
            IDictionary<int, int> cart = new Dictionary<int, int>();
            cart = JsonSerializer.Deserialize<Dictionary<int, int>>(cart1);
            Order order1 = context.Orders.Where(o => o.Purchases == count).SingleOrDefault();
            foreach (KeyValuePair<int, int> item in cart)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Idorder = order1.Id;
                orderDetail.Product = item.Key;
                orderDetail.Number = item.Value;
                context.OrderDetails.Add(orderDetail);
                context.SaveChanges();
            }
            cart = new Dictionary<int, int>();
            string jsonData = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("cart", jsonData);
            int sum1 = 0;
            foreach (KeyValuePair<int, int> item in cart)
            {
                sum1 += item.Value;
            }
            HttpContext.Session.SetString("slsp", sum1.ToString());
            return RedirectToAction("HomePage");
        }
        public IActionResult ManageProduct()
        {
            using var context = new WebShopContext();
            List<Product> list = context.Products.ToList();
            ViewBag.list = list;
            return View();
        }

        public IActionResult AnHien(int id, int type)
        {
            using var context = new WebShopContext();
            Product product = context.Products.Where(x => x.Id == id).SingleOrDefault();
            if (type == 0)
            {
                product.Status = false;
            }
            else
            {
                product.Status = true;
            }
            context.Products.Update(product);
            context.SaveChanges();
            return Redirect("ManageProduct");
        }
        public IActionResult AddProduct()
        {
            return View();
        }
        public IActionResult AddNewProduct()
        {
            string name = HttpContext.Request.Form["name"];
            string image = HttpContext.Request.Form["image"];
            string price = HttpContext.Request.Form["price"];
            string number = HttpContext.Request.Form["number"];
            using var context = new WebShopContext();
            Product product = new Product();
            product.ProductName = name;
            product.Image = image;
            product.Number = int.Parse(number);
            product.Price = int.Parse(price);
            product.Status = true;
            context.Products.Add(product);
            context.SaveChanges();
            return Redirect("ManageProduct");
        }
        public IActionResult FormEdit(int id)
        {
            using var context = new WebShopContext();
            Product product = context.Products.Where(x => x.Id == id).SingleOrDefault();
            ViewBag.product = product;
            return View();
        }
        [HttpPost]
        public IActionResult EditProduct(int id)
        {
            string name = HttpContext.Request.Form["pname"];
            string image = HttpContext.Request.Form["image"];
            string price = HttpContext.Request.Form["price"];
            string number = HttpContext.Request.Form["number"];
            using var context = new WebShopContext();
            Product product = context.Products.Where(x => x.Id == id).SingleOrDefault();
            product.ProductName = name;
            product.Image = image;
            product.Number = int.Parse(number);
            product.Price = int.Parse(price);
            product.Status = true;
            context.Products.Update(product);
            context.SaveChanges();
            return Redirect("ManageProduct");
        }

        public IActionResult ManageOrder()
        {
            using (var context = new WebShopContext())
            {
                List<Order> list = context.Orders.ToList();
                foreach (Order item in list)
                {
                    item.CustomerNavigation = context.Accounts.Where(x => x.Id == item.Customer).Single();
                }
                ViewBag.list = list;
            }
            return View();
        }

        public IActionResult ManageAccount()
        {
            using var context = new WebShopContext();
            List<Account> list = context.Accounts.ToList();
            ViewBag.list = list;
            return View();
        }

        public IActionResult FormEditAccount(int id)
        {
            using var context = new WebShopContext();
            Account account = context.Accounts.Where(x => x.Id == id).SingleOrDefault();
            ViewBag.account = account;
            return View();
        }

        public IActionResult EditAccount(int id)
        {
            string name = HttpContext.Request.Form["name"];
            string email = HttpContext.Request.Form["email"];
            string phone = HttpContext.Request.Form["phone"];
            using var context = new WebShopContext();
            Account account = context.Accounts.Where(x => x.Id == id).SingleOrDefault();
            account.Fullname = name;
            account.Email = email;
            account.Phone = phone;
            context.Accounts.Update(account);
            context.SaveChanges();
            return Redirect("ManageAccount");
        }

        public IActionResult ViewProfile(int id)
        {
            using var context = new WebShopContext();
            Account account = context.Accounts.Where(x => x.Id == id).SingleOrDefault();
            ViewBag.account = account;
            return View();
        }

        public IActionResult FormEditProfile(int id)
        {
            using var context = new WebShopContext();
            Account account = context.Accounts.Where(x => x.Id == id).SingleOrDefault();
            ViewBag.account = account;
            return View();
        }
        [HttpPost]
        public IActionResult EditProfile(int id)
        {
            string name = HttpContext.Request.Form["fname"];
            string email = HttpContext.Request.Form["email"];
            string phone = HttpContext.Request.Form["phone"];
            string user = HttpContext.Request.Form["user"];
            using var context = new WebShopContext();
            Account account = context.Accounts.Where(x => x.Id == id).SingleOrDefault();
            account.Fullname = name;
            account.Email = email;
            account.Phone = phone;
            account.Username = user;
            context.Accounts.Update(account);
            context.SaveChanges();
            return Redirect("ViewProfile?id=" + id);
        }

        public IActionResult FormEditPassword(int id, string message)
        {
            using var context = new WebShopContext();
            Account account = context.Accounts.Where(x => x.Id == id).SingleOrDefault();
            ViewBag.account = account;
            ViewBag.message = message;
            return View();
        }
        public IActionResult EditPassword(int id)
        {
            string oldpassword = HttpContext.Request.Form["oldpass"];
            string newpassword = HttpContext.Request.Form["newpass"];
            string cnewpassword = HttpContext.Request.Form["cnewpass"];
            using var context = new WebShopContext();
            Account account = context.Accounts.Where(x => x.Id == id && x.Password == oldpassword).SingleOrDefault();
            string message;
            if (account == null)
            {
                message = "Mat khau khong dung";
                ViewBag.message = message;
            }
            else
            {
                if (newpassword.Equals(cnewpassword))
                {
                    account.Password = newpassword;
                    context.Accounts.Update(account);
                    context.SaveChanges();
                    message = "Thay doi mat khau thanh cong";
                    ViewBag.message = message;
                }
                else
                {
                    message = "nhap lai cf password";
                    ViewBag.message = message;
                }
            }
            return Redirect("FormEditPassWord?id=" + id+"&message="+message);
        }
        public IActionResult ViewOrder(int id)
        {
            return View();
        }
        
        }
}
