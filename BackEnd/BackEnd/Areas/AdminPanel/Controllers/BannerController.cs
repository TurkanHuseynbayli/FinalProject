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
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class BannerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public BannerController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            Banner banner = _context.Banners.FirstOrDefault();
            return View(banner);
        }

        public IActionResult Update(int? id)
        {
            Banner banner = _context.Banners.FirstOrDefault(a => a.Id == id);
            return View(banner);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Banner banner)
        {
            Banner bannerOld = _context.Banners.FirstOrDefault(a => a.Id == id);
            if (id == null) return NotFound();
            if (banner == null) return NotFound();

            if (banner.Photo != null)
            {
                if (!banner.Photo.IsImage())
                {
                    ModelState.AddModelError("Photos", $"{banner.Photo.FileName} - not image type");
                    return View(bannerOld);
                }

                string folder = Path.Combine("img", "banner");
                string fileName = await banner.Photo.SaveImageAsync(_env.WebRootPath, folder);
                if (fileName == null)
                {
                    return Content("Error");
                }

                Helper.DeleteImage(_env.WebRootPath, folder, bannerOld.Image);
                bannerOld.Image = fileName;
            }


            bannerOld.Description = banner.Description;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
