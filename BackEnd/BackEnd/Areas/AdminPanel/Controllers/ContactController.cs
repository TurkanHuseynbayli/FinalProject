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
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ContactController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        

            #region Index
            public IActionResult Index()
            {
                Contact contact = _context.Contacts.FirstOrDefault();
                return View(contact);
            }
        #endregion

        #region Update
        public IActionResult Update(int? id)
        {
            Contact contact = _context.Contacts.FirstOrDefault(a => a.Id == id);
            return View(contact);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Contact contact)
        {
            Contact contactOld = _context.Contacts.FirstOrDefault(a => a.Id == id);
            if (id == null) return NotFound();
            if (contact == null) return NotFound();

            if (contact.Photo != null)
            {
                if (!contact.Photo.IsImage())
                {
                    ModelState.AddModelError("Photos", $"{contact.Photo.FileName} - not image type");
                    return View(contactOld);
                }

                string folder = Path.Combine("img", "contact");
                string fileName = await contact.Photo.SaveImageAsync(_env.WebRootPath, folder);
                if (fileName == null)
                {
                    return Content("Error");
                }

                Helper.DeleteImage(_env.WebRootPath, folder, contactOld.Image);
                contactOld.Image = fileName;
            }


            contactOld.Email = contact.Email;
            contactOld.Phone = contact.Phone;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

    }
}
