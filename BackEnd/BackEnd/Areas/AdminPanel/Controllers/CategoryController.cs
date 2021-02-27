using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DAL;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Categories.Where(c => c.IsDeleted == false).ToList());
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {


            if (!ModelState.IsValid) return NotFound();
            bool isExist = _context.Categories.Where(c => c.IsDeleted == false).Any(c => c.NameCategory.ToLower() == category.NameCategory.ToLower());
            if (isExist)
            {
                ModelState.AddModelError("NameCategory", "bu addan var");
                return View();
            }
            category.IsDeleted = false;
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detail(int? id)
        {

            if (id == null) return NotFound();
            Category category = _context.Categories.Where(c => c.IsDeleted == false).FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            Category category = _context.Categories.Where(c => c.IsDeleted == false).FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null) return NotFound();
            Category category = _context.Categories.Where(c => c.IsDeleted == false).Include(c => c.CategoryProducts).ThenInclude(c=>c.Product).FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            category.IsDeleted = true;
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

           
            //foreach (Product pro in category.CategoryProducts.)
            //{
            //    //pro.DeletedTime = DateTime.Now;
            //    pro.IsDeleted = true;
            //}
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            if (id == null) return NotFound();
            Category category = _context.Categories.Where(c => c.IsDeleted == false).FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Update")]
        public async Task<IActionResult> Update(int? id, Category category)
        {

            if (id == null) return NotFound();
            if (category == null) return NotFound();
            Category categ = await _context.Categories.FindAsync(id);
            Category isExist = _context.Categories.Where(c => c.IsDeleted == false).FirstOrDefault(c => c.NameCategory.ToLower() == category.NameCategory.ToLower());
            if (isExist != null)
            {
                if (isExist != categ)
                {
                    ModelState.AddModelError("NameCategory", "Artiq bu adda category movcuddur");
                    return View();
                }
            }
            categ.NameCategory = category.NameCategory;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

    }
}

