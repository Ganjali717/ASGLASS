using ASGlass.DAL;
using ASGlass.Models;
using ASGlass.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ASGlass.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ContactViewModel contactVM = new ContactViewModel()
            {
                Setting = _context.Settings.FirstOrDefault()
            };
            return View(contactVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(EmailForm model)
        {
            
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
                message.From = new MailAddress(HttpContext.Request.Form["email"].ToString());
                message.To.Add(new MailAddress("anar.aliyev717@gmail.com"));
                message.Subject = "Müştəri";
                message.Body = HttpContext.Request.Form["textarea"];
                message.IsBodyHtml = true;

                await smtp.SendMailAsync(message);
            
            return View(model);
        }
    }
}
