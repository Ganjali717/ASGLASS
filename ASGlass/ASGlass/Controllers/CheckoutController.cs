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

        public IActionResult Index()
        {
            var value = HttpContext.Request.Cookies["Products"];

            if (value != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("index", "card");
            }

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


                        var message = new MailMessage();
                        message.From = new MailAddress("anar.aliyev717@gmail.com");
                        message.To.Add(new MailAddress(orders.Email));
                        message.Subject = "Sifaris kodunuz";
                        message.Body = "Sifaris kodunuz: " + Convert.ToString(orders.OrderNumber);
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
                        orders.FullName = HttpContext.Request.Form["fullname"].ToString();
                        orders.Email = HttpContext.Request.Form["email"].ToString();
                        orders.ProductName = item.Name;
                        orders.Price = item.Price;
                        orders.ProductImage = item.Image;
                        orders.CreatedAt = DateTime.UtcNow;
                        orders.Status = Models.Enums.OrderStatus.Pending;
                        orders.OrderNumber = random.Next(10000, 90000);
                    }

                    _context.CartItems.Remove(item);


                    if (item.ProductId != null)
                    {
                        Product product = _context.Products.FirstOrDefault(x => x.Id == item.ProductId);
                        List<CartItem> cartItem = _context.CartItems.Where(x => x.AppUserId == member.Id && x.ProductId == item.ProductId).ToList();

                        _context.Products.Remove(product);
                    }

                     SmtpClient smtp = new SmtpClient();

                    /*var credential = new NetworkCredential
                    {
                        UserName = "anar.aliyev717@gmail.com",
                        Password = "genceli717"
                    };*/
                    smtp.Credentials = CredentialCache.DefaultNetworkCredentials;
                    smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                    

                    var message = new MailMessage();
                    message.From = new MailAddress("anar.aliyev717@gmail.com");
                    message.To.Add(new MailAddress(orders.Email));
                    message.Subject = "Sifaris kodunuz";
                    message.Body = Convert.ToString(orders.OrderNumber);
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
