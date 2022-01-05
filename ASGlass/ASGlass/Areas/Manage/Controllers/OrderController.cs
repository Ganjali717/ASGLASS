using ASGlass.DAL;
using ASGlass.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ASGlass.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "Admin, SuperAdmin")]

    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ASGlassHub> _hubContext;

        public OrderController(AppDbContext context, IHubContext<ASGlassHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            List<Order> orders = _context.Orders.Include(x => x.Product).ToList();
            return View(orders);
        }

        public IActionResult Edit(int id)
        {
            Order order = _context.Orders.Include(x => x.Product).FirstOrDefault(x => x.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }
        public async Task<IActionResult> Accept(int id)
        {
            Order order = _context.Orders.Include(x => x.Product).FirstOrDefault(x => x.Id == id);
            if (order == null) return NotFound();

            order.Status = Models.Enums.OrderStatus.Accepted;
            _context.SaveChanges();

            if (order.AppUser?.ConnectionId != null)
            {
                await _hubContext.Clients.Client(order.AppUser.ConnectionId).SendAsync("OrderAccepted");
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
            message.To.Add(new MailAddress(order.Email));
            message.Subject = "Sifaris kodunuz";
            message.Body = "Sizin " + Convert.ToString(order.OrderNumber) + " kodlu sifariwiniz qebul edilmiwdir yaxin zamanda sifariwiniz hazir olacaqdir. Elaqede qalin. Bizi secdiyiniz ucun size tesekkur edirik :)";
            message.IsBodyHtml = true;

            await smtp.SendMailAsync(message);

            return RedirectToAction("index");
        }

        public async Task<IActionResult> Ready(int id)
        {
            Order order = _context.Orders.Include(x => x.Product).FirstOrDefault(x => x.Id == id);
            if (order == null) return NotFound();

            order.Status = Models.Enums.OrderStatus.Ready;
            _context.SaveChanges();
             

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
            message.To.Add(new MailAddress(order.Email));
            message.Subject = "Sifaris kodunuz";
            message.Body = "Hormetli" + order.FullName + Convert.ToString(order.OrderNumber) + " kodlu sifariwiniz hazirdir";
            message.IsBodyHtml = true;

            await smtp.SendMailAsync(message);

            return RedirectToAction("index");
        }

        public IActionResult Reject(int id)
        {
            Order order = _context.Orders.Include(x => x.Product).FirstOrDefault(x => x.Id == id);
            if (order == null) return NotFound();

            order.Status = Models.Enums.OrderStatus.Rejected;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult DeleteFetch(int id)
        {
            Order order = _context.Orders.Include(x => x.Product).FirstOrDefault(x => x.Id == id);

            if (order == null) return Json(new { status = 404 });

            try
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { status = 500 });
            }

            return Json(new { status = 200 });
        }
    }
}
