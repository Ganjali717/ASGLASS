using ASGlass.DAL;
using ASGlass.Models;
using ASGlass.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ASGlass.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CheckoutController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var value = HttpContext.Request.Cookies["Products"];

            AppUser member = null;
            if (User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if(member == null)
            {
                if (string.IsNullOrWhiteSpace(value) || value == "[]")
                {
                    return RedirectToAction("index", "card");
                }
            }
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            List<CartViewModel> products = new List<CartViewModel>();

            AppUser member = null;
            if (User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(User.Identity.Name);
            }

            if (member == null)
            {
                var productStr = HttpContext.Request.Cookies["Products"];
                if (!string.IsNullOrWhiteSpace(productStr))
                {
                    products = JsonConvert.DeserializeObject<List<CartViewModel>>(productStr);

                    foreach (var item in products)
                    {
                        Order orders = new Order();
                        Random random = new Random();
                        if (products != null)
                        {
                            orders.ProductId = item.ProductId;
                            orders.FullName = HttpContext.Request.Form["fullname"].ToString();
                            orders.Email = HttpContext.Request.Form["email"].ToString();
                            orders.ProductName = item.Name;
                            orders.Price = item.Price;
                            orders.ProductImage = item.Image;
                            orders.CreatedAt = DateTime.UtcNow;
                            orders.Status = Models.Enums.OrderStatus.Pending;
                            orders.OrderNumber = random.Next(10000,90000);
                            orders.Polish = item.Polish;
                            orders.Corner = item.Corner;
                            orders.Uzunluq = item.Uzunluq;
                            orders.En = item.En;
                            orders.Color = item.Color;
                            orders.Count = item.Count;
                            orders.Diametr = item.Diametr;
                            orders.Thickness = item.Thickness;
                        }

                        if (item.ProductId != null)
                        {
                            Product product = _context.Products.FirstOrDefault(x => x.Id == item.ProductId);
                            _context.Products.Remove(product);
                        }

                        SmtpClient smtp = new SmtpClient();

                        var credential = new NetworkCredential
                        {
                            UserName = "anar.aliyev717@gmail.com",
                            Password = "genceli717"
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;

                        string callback = "https://localhost:44393/service/orderstatus";

                        var message = new MailMessage();
                        message.From = new MailAddress("anar.aliyev717@gmail.com");
                        message.To.Add(new MailAddress(orders.Email));
                        message.Subject = "Sifaris kodunuz";
                        message.Body = "Sifaris kodunuz: " + Convert.ToString(orders.OrderNumber) + "  Sifariwinizi buradan yoxlaya bilersiniz: " + callback;
                        message.IsBodyHtml = true;

                        await smtp.SendMailAsync(message);


                        _context.Orders.Add(orders);
                    }
                }
                HttpContext.Response.Cookies.Delete("Products");
            }
            else
            {
                List<CartItem> cartItems = _context.CartItems.Include(x => x.Product.ProductImages).Include(x => x.Product).Include(x => x.Product.Shape).Include(x => x.Product.Colors).Where(x => x.AppUserId == member.Id).ToList();

                foreach (var item in cartItems)
                {
                    Order orders = new Order();
                    Random random = new Random();
                    if (cartItems != null)
                    {
                        orders.AppUserId = item.AppUserId;
                        orders.ProductId = item.ProductId != null ? item.ProductId:null;
                        orders.FullName = member.FullName;
                        orders.Email = member.Email;
                        orders.ProductName = item.Name;
                        orders.Price = item.Price;
                        orders.ProductImage = item.Image;
                        orders.CreatedAt = DateTime.UtcNow;
                        orders.Status = Models.Enums.OrderStatus.Pending;
                        orders.OrderNumber = random.Next(10000, 90000);
                        orders.Polish = item.Polish;
                        orders.Corner = item.Corner;
                        orders.Uzunluq = item.Uzunluq;
                        orders.En = item.En;
                        orders.Color = item.Color;
                        orders.Count = item.Count;
                        orders.Diametr = item.Diametr;
                        orders.Thickness = item.Thickness;
                    }

                    _context.CartItems.Remove(item);


                    if (item.ProductId != null)
                    {
                        Product product = _context.Products.FirstOrDefault(x => x.Id == item.ProductId);
                        List<CartItem> cartItem = _context.CartItems.Where(x => x.AppUserId == member.Id && x.ProductId == item.ProductId).ToList();

                        _context.Products.Remove(product);
                    }

                     SmtpClient smtp = new SmtpClient();

                    var credential = new NetworkCredential
                    {
                        UserName = "anar.aliyev717@gmail.com",
                        Password = "genceli717"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;

                    string callback = "https://localhost:44393/service/orderstatus";


                    var message = new MailMessage();
                    message.From = new MailAddress("anar.aliyev717@gmail.com");
                    message.To.Add(new MailAddress(orders.Email));
                    message.Subject = "Sifaris kodunuz";
                    message.Body = "Sifaris kodunuz: " + Convert.ToString(orders.OrderNumber) + "     Sifariwinizi buradan yoxlaya bilersiniz: " + callback;
                    message.IsBodyHtml = true;

                   await smtp.SendMailAsync(message);

                    _context.Orders.Add(orders);
                }
            }

           
            _context.SaveChanges();
            return Redirect(HttpContext.Request.Headers["Referer"].ToString());
        }
    }
}
