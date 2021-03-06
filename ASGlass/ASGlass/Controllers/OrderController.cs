using ASGlass.DAL;
using ASGlass.Models;
using ASGlass.Models.Enums;
using ASGlass.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGlass.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult Index()
        {
            var query = _context.Orders.Include(x => x.Product).AsQueryable();

            long? ordercode = Convert.ToInt64(HttpContext.Request.Form["orderstatus"]);

            if(ordercode != null)
            {
                if (query.Any(x => x.OrderNumber == ordercode))
                {
                    query = query.Include(x => x.Product).Where(x => x.OrderNumber == ordercode);
                }
                else
                {
                    return RedirectToAction("orderstatus", "service");

                }
            }


            OrderViewModel orderVM = new OrderViewModel()
            {
                ProductName = query.FirstOrDefault().ProductName,
                ProductImage = query.FirstOrDefault().ProductImage,
                Status = query.FirstOrDefault().Status,
                Price = query.FirstOrDefault().Price,
                CreatedAt = query.FirstOrDefault().CreatedAt
            };
            return View(orderVM);

        }


        public IActionResult DeleteFromOrder(int id)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == id);

            if (order == null) return NotFound();

            if(order.Status == OrderStatus.Rejected || order.Status == OrderStatus.Pending)
            {
                _context.Orders.Remove(order);
            }
            else
            {
                return BadRequest();
            }

            _context.SaveChanges();

            return RedirectToAction("index", "order");
        }


    }
}
