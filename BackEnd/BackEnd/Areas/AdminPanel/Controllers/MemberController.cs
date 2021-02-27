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
    public class MemberController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public MemberController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            ViewBag.MemberCount = _context.Members.Count();
            return View(_context.Members.ToList());
        }

        #region Member Create
        public IActionResult Create()
        {
            int count = _context.Members.Count();
            if (count >= 5)
            {
                return Content("sile bilmezsiz");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Member member)
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
            if (member.Photos == null)
            {
                return View();
            }
            int count = _context.Members.Count();
            if (count + member.Photos.Length > 5)
            {
                ModelState.AddModelError("Photos", $"{5 - count}- image can select");
                return View();
            }
            foreach (IFormFile photo in member.Photos)
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
                Member newMember = new Member();
                string path = Path.Combine("img", "team");
                newMember.Image = await photo.SaveImageAsync(_env.WebRootPath, path);
                newMember.Name = member.Name;
                newMember.Description = member.Description;
                newMember.Specialty = member.Specialty;

                await _context.Members.AddAsync(newMember);


                //Slider newSlider = new Slider();
                //newSlider.Image = await photo.SaveImageAsync(_env.WebRootPath, "img",);
                await _context.Members.AddAsync(newMember);
            }

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));

        }
        #endregion Member Create

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            Member member = _context.Members.FirstOrDefault(c => c.Id == id);
            if (member == null) return NotFound();
            await _context.SaveChangesAsync();
            return View(member);

        }
        #region member Update
        public async Task<IActionResult> Update(int? id)
        {

            if (id == null) return NotFound();
            Member member = _context.Members.FirstOrDefault(c => c.Id == id);
            if (member == null) return NotFound();
            await _context.SaveChangesAsync();
            return View(member);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Update")]
        public async Task<IActionResult> Update(Member member, int? id)
        {

            if (id == null) return NotFound();
            if (member == null) return NotFound();

            Member member1 = await _context.Members.FindAsync(id);

            if (!member.Photo.IsImage())
            {
                ModelState.AddModelError("Photos", $"{member.Photo.FileName} - not image type");
                return View(member1);
            }
            if (!member.Photo.MaxSize(200))
            {
                ModelState.AddModelError("Photos", $"{member.Photo.FileName} - Max image length must be less than 200kb");
                return View(member1);
            }


            string folder = Path.Combine("img", "team");
            string fileName = await member.Photo.SaveImageAsync(_env.WebRootPath, folder);
            if (fileName == null)
            {
                return Content("Error");
            }

            member1.Image = fileName;
            member1.Name = member.Name;
            member1.Specialty = member.Specialty;
            member1.Description = member.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        #endregion Member Update


        #region Member Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Member member = await _context.Members.FindAsync(id);
            if (member == null) return NotFound();
            int count = _context.Members.Count();
            if (count == 1)
            {
                return Content("sile bilmezsiz");
            }
            return View(member);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {

            if (id == null) return NotFound();
            Member member = _context.Members.FirstOrDefault(c => c.Id == id);
            if (member == null) return NotFound();
            int count = _context.Members.Count();
            if (count == 1)
            {
                return Content("sile bilmezsiz");
            }
            bool isDeleted = Helper.DeleteImage(_env.WebRootPath, "img", member.Image);
            if (!isDeleted)
            {
                ModelState.AddModelError(" ", "Some problem exists");
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        #endregion Member Delete
    }
}
