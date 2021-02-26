using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DAL;
using BackEnd.Extensions;
using BackEnd.Helpers;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
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
            ViewBag.SlideCount = _context.Sliders.Count();
            return View(_context.Sliders.ToList());
        }
        #region Slider Create
        public IActionResult Create()
        {
            int count = _context.Sliders.Count();
            if (count >= 5)
            {
                return Content("sile bilmezsiz");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            #region file upload
            //if (slider.Photo == null)
            //{
            //    return View();
            //}
            //if (!slider.Photo.IsImage())
            //{
            //    ModelState.AddModelError("Photo", "Please select image type");
            //    return View();
            //}
            //if (!slider.Photo.MaxSize(200))
            //{
            //    ModelState.AddModelError("Photo", "Max 200kb");
            //    return View();
            //}

            //int count = _context.Sliders.Count();
            //if (count >= 5)
            //{
            //    return Content("sile bilmezsiz");
            //}


            //slider.Image = await slider.Photo.SaveImageAsync(_env.WebRootPath, "img");

            //await _context.Sliders.AddAsync(slider);
            //await _context.SaveChangesAsync();
            #endregion
            if (slider.Photos == null)
            {
                return View();
            }
            int count = _context.Sliders.Count();
            if (count + slider.Photos.Length > 5)
            {
                ModelState.AddModelError("Photos", $"{5 - count}- image can select");
                return View();
            }
            foreach (IFormFile photo in slider.Photos)
            {

                if (!photo.IsImage())
                {
                    ModelState.AddModelError("Photo", $"{photo.FileName}-not image type");
                    return View();
                }
                if (!photo.MaxSize(200))
                {
                    ModelState.AddModelError("Photo", $"{photo.FileName}-max length 200kb");
                    return View();
                }
                Slider newSlider = new Slider();
                string path = Path.Combine("img", "slider");
                newSlider.Image = await photo.SaveImageAsync(_env.WebRootPath, path);
                newSlider.SubTitle = slider.SubTitle;
                newSlider.Title = slider.Title;
                
                await _context.Sliders.AddAsync(newSlider);


                //Slider newSlider = new Slider();
                //newSlider.Image = await photo.SaveImageAsync(_env.WebRootPath, "img",);
                //await _context.Sliders.AddAsync(newSlider);
            }

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));

        }
        #endregion Slider Create

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            Slider slider = _context.Sliders.FirstOrDefault(c => c.Id == id);
            if (slider == null) return NotFound();
            await _context.SaveChangesAsync();
            return View(slider);

        }

        #region Slider Update
        public async Task<IActionResult> Update(int? id)
        {

            if (id == null) return NotFound();
            Slider slider = _context.Sliders.FirstOrDefault(c => c.Id == id);
            if (slider == null) return NotFound();
            await _context.SaveChangesAsync();
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Update")]
        public async Task<IActionResult> Update(Slider slider, int? id)
        {

            if (id == null) return NotFound();
            if (slider == null) return NotFound();

            Slider slide = await _context.Sliders.FindAsync(id);

            if (!slider.Photo.IsImage())
            {
                ModelState.AddModelError("Photos", $"{slider.Photo.FileName} - not image type");
                return View(slide);
            }
            if (!slider.Photo.MaxSize(200))
            {
                ModelState.AddModelError("Photos", $"{slider.Photo.FileName} - Max image length must be less than 200kb");
                return View(slide);
            }


            string folder = Path.Combine("img", "slider");
            string fileName = await slider.Photo.SaveImageAsync(_env.WebRootPath, folder);
            if (fileName == null)
            {
                return Content("Error");
            }
            
            slide.Image = fileName;
            slide.SubTitle = slider.SubTitle;
            slide.Title = slider.Title;
          
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        #endregion Slider Update

        #region Slider Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Slider slider = await _context.Sliders.FindAsync(id);
            if (slider == null) return NotFound();
            int count = _context.Sliders.Count();
            if (count == 1)
            {
                return Content("sile bilmezsiz");
            }
            return View(slider);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {

            if (id == null) return NotFound();
            Slider slider = _context.Sliders.FirstOrDefault(c => c.Id == id);
            if (slider == null) return NotFound();
            int count = _context.Sliders.Count();
            if (count == 1)
            {
                return Content("sile bilmezsiz");
            }
            bool isDeleted = Helper.DeleteImage(_env.WebRootPath, "img", slider.Image);
            if (!isDeleted)
            {
                ModelState.AddModelError(" ", "Some problem exists");
            }

            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        #endregion Slider Delete
    }
}
