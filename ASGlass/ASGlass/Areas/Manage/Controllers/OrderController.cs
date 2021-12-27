using ASGlass.DAL;
using ASGlass.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
