using ASGlass.DAL;
using ASGlass.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASGlass.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles="Admin, SuperAdmin")]
    public class AboutController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AboutController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            var about = _context.Abouts.FirstOrDefault();
            return View(about);
        }

        public IActionResult Edit(int id)
        {
            if (!ModelState.IsValid) return NotFound();

            var about = _context.Abouts.FirstOrDefault(x => x.Id == id);
            return View(about);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(About about)
        {
            About existAbout = _context.Abouts.FirstOrDefault(x => x.Id == about.Id);

            if (existAbout == null) return NotFound();


            string newFileName = null;
            if (about.PosterFile != null)
            {
                if (about.PosterFile.ContentType != "image/png" && about.PosterFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "File type can be only jpeg,jpg or png!");
                    return View();
                }

                if (about.PosterFile.Length > 222097152)
                {
                    ModelState.AddModelError("ImageFile", "File size can not be more than 2MB!");
                    return View();
                }

                newFileName = Guid.NewGuid().ToString() + about.PosterFile.FileName;
                string path = Path.Combine(_env.WebRootPath, "uploads/aboutus", newFileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    about.PosterFile.CopyTo(stream);
                }
            }

            if (newFileName != null || about.Image == null)
            {
                if (existAbout.Image != null)
                {
                    string deletePath = Path.Combine(_env.WebRootPath, "uploads/slider", existAbout.Image);

                    if (System.IO.File.Exists(deletePath))
                    {
                        System.IO.File.Delete(deletePath);
                    }
                }

                existAbout.Image = newFileName;
            }


            existAbout.Title = about.Title;

            _context.SaveChanges();
            return RedirectToAction("index", "about");
        }
    }
}
