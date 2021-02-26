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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class BiosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;
        public BiosController(AppDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            Bio bios = _context.Bios.FirstOrDefault();
            return View(bios);
        }
        public IActionResult Update(int? id)
        {
            Bio bio = _context.Bios.FirstOrDefault(bio => bio.Id == id);

            return View(bio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Bio bios)
        {
            Bio oldBio = _context.Bios.FirstOrDefault(bio => bio.Id == id);
            if (bios.Photo != null)
            {
                if (!bios.Photo.IsImage())
                {
                    ModelState.AddModelError("Photos", $"{bios.Photo.FileName} - not image type");
                    return View(oldBio);
                }

                string folder = Path.Combine("img", "logo");
                string fileName = await bios.Photo.SaveImageAsync(_env.WebRootPath, folder);
                if (fileName == null)
                {
                    return Content("Error");
                }

                Helper.DeleteImage(_env.WebRootPath, folder, oldBio.Logo);
                oldBio.Logo = fileName;
            }
            if (bios.Photo != null)
            {
                if (!bios.Photo.IsImage())
                {
                    ModelState.AddModelError("Photos", $"{bios.Photo.FileName} - not image type");
                    return View(oldBio);
                }

                string folder = Path.Combine("img", "logo");
                string fileName = await bios.Photo.SaveImageAsync(_env.WebRootPath, folder);
                if (fileName == null)
                {
                    return Content("Error");
                }

                Helper.DeleteImage(_env.WebRootPath, folder, oldBio.Logo);
                oldBio.Logo = fileName;
            }

            oldBio.Address = bios.Address;
            oldBio.Phono = bios.Phono;
            oldBio.Email = bios.Email;
         
          
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
