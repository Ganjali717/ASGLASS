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
    [Authorize(Roles = "Admin, SuperAdmin")]

    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            List<Slider> slider = _context.Sliders.ToList();
            return View(slider);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Slider slider)
        {

            if (slider.PosterFile != null)
            {

                if (slider.PosterFile.ContentType != "image/png" && slider.PosterFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("PosterFile", "File type can be only jpeg,jpg or png!");
                    return View();
                }

                if (slider.PosterFile.Length > 222097152)
                {
                    ModelState.AddModelError("Posterfile", "File size can not be more than 2MB!");
                    return View();
                }

                string newFileName = Guid.NewGuid().ToString() + slider.PosterFile.FileName;
                string path = Path.Combine(_env.WebRootPath, "uploads/slider", newFileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    slider.PosterFile.CopyTo(stream);
                }

                slider.Image = newFileName;
            }
            else
            {
                ModelState.AddModelError("PosterFile", "Poster file is required");
            }


            _context.Sliders.Add(slider);
            _context.SaveChanges();

            return RedirectToAction("index");
        }


        public IActionResult Edit(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);

            if (slider == null) return NotFound();

            return View(slider);
        }

        [HttpPost]
        public IActionResult Edit(Slider slider)
        {
            if (!ModelState.IsValid) return View();

            Slider existSlider = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);

            if (existSlider == null) return NotFound();

            string newFileName = null;
            if (slider.PosterFile != null)
            {
                if (slider.PosterFile.ContentType != "image/png" && slider.PosterFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "File type can be only jpeg,jpg or png!");
                    return View();
                }

                if (slider.PosterFile.Length > 222097152)
                {
                    ModelState.AddModelError("ImageFile", "File size can not be more than 2MB!");
                    return View();
                }

                newFileName = Guid.NewGuid().ToString() + slider.PosterFile.FileName;
                string path = Path.Combine(_env.WebRootPath, "uploads/slider", newFileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    slider.PosterFile.CopyTo(stream);
                }
            }

            if (newFileName != null || slider.Image == null)
            {
                if (existSlider.Image != null)
                {
                    string deletePath = Path.Combine(_env.WebRootPath, "uploads/slider", existSlider.Image);

                    if (System.IO.File.Exists(deletePath))
                    {
                        System.IO.File.Delete(deletePath);
                    }
                }

                existSlider.Image = newFileName;
            }

            existSlider.Title = slider.Title;
            existSlider.SubTitle = slider.SubTitle;
            existSlider.RedirectUrl = slider.RedirectUrl;
            existSlider.Order = slider.Order;

            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult DeleteFetch(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);

            if (slider == null) return Json(new { status = 404 });

            try
            {
                _context.Sliders.Remove(slider);
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
