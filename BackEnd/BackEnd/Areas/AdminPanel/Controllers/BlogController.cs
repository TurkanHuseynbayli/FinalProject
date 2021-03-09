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
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class BlogController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BlogController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int? page = 1)
        {
            ViewBag.PageCount = Decimal.Ceiling((decimal)_context.Products
             .Where(pro => pro.IsDeleted == false).Count() / 9);
            ViewBag.Page = page;
            List<Blog> blogs = _context.Blogs.Where(blg => blg.isDelete == false)
               .Include(blg => blg.BlogDetail).OrderByDescending(blg => blg.Date).Skip(((int)page - 1) * 3).Take(4).ToList();
            return View(blogs);
        }
        public async Task<IActionResult> Detail(int? id)
        {
            Blog blog = await _context.Blogs.Include(blg => blg.BlogDetail).FirstOrDefaultAsync(blg => blg.Id == id);
            return View(blog);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blogs)
        {
           
            Blog newBlog = new Blog();
            BlogDetail newBlogDetail = new BlogDetail();


            if (blogs.Name == null)
            {
                ModelState.AddModelError("Name", "Name cannot be empty");
                return View();
            }

            if (blogs.Photo == null)
            {
                ModelState.AddModelError("Photo", "Image cannot be empty");
                return View();
            }




            if (!blogs.Photo.IsImage())
            {
                ModelState.AddModelError("Photos", $"{blogs.Photo.FileName} - not image type");
                return View(newBlog);
            }

            string folder = Path.Combine("img", "blog");
            string fileName = await blogs.Photo.SaveImageAsync(_env.WebRootPath, folder);
            if (fileName == null)
            {
                return Content("Error");
            }

            newBlog.Image = fileName;
            newBlog.Name = blogs.Name;
            newBlog.Date = blogs.Date;
            await _context.AddAsync(newBlog);
            await _context.SaveChangesAsync();


            newBlogDetail.Description = blogs.BlogDetail.Description;



            newBlogDetail.BlogId = newBlog.Id;
            await _context.AddAsync(newBlogDetail);
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));

        }

        #region Update
        public IActionResult Update(int? id)
        {
            ViewBag.Categ = _context.Categories.ToList();
            
            Blog blogs = _context.Blogs.Include(blg => blg.BlogDetail).FirstOrDefault(blg => blg.Id == id);
            return View(blogs);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Blog blog)
        {
            ViewBag.Categ = _context.Categories.ToList();
            if (id == null) return NotFound();


            Blog blogOld = await _context.Blogs.Include(c => c.BlogDetail).FirstOrDefaultAsync(c => c.Id == id);
            Blog isExist = _context.Blogs.Where(cr => cr.isDelete == false).FirstOrDefault(cr => cr.Id == id);
            bool exist = _context.Blogs.Where(cr => cr.isDelete == false).Any(cr => cr.Name == blog.Name);

            if (exist)
            {
                if (isExist.Name != blog.Name)
                {
                    ModelState.AddModelError("Name", "This name already has. Please write another name");
                    return View(blogOld);
                }
            }

            if (blog == null) return Content("Null");
            if (blog.Photo != null)
            {
                if (!blog.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", $"{blog.Photo.FileName} - not image type");
                    return View(blogOld);
                }

                string folder = Path.Combine("img", "blog");
                string fileName = await blog.Photo.SaveImageAsync(_env.WebRootPath, folder);
                if (fileName == null)
                {
                    return Content("Error");
                }

                Helper.DeleteImage(_env.WebRootPath, folder, blogOld.Image);
                blogOld.Image = fileName;
            }


            #region Update line
            blogOld.Name = blog.Name;
            blogOld.BlogDetail.Description = blog.BlogDetail.Description;
            #endregion

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Blog blog = await _context.Blogs.FindAsync(id);
            if (blog == null) return NotFound();
            int count = _context.Blogs.Count();
            if (count == 1)
            {
                return Content("sile bilmezsiz");
            }
            return View(blog);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {

            if (id == null) return RedirectToAction("Index", "Error");
            Blog blog = _context.Blogs.FirstOrDefault(c => c.Id == id);
            if (blog == null) return NotFound();
            int count = _context.Blogs.Count();
            if (count == 1)
            {
                return Content("sile bilmezsiz");
            }
            bool isDeleted = Helper.DeleteImage(_env.WebRootPath, "img", blog.Image);
            if (!isDeleted)
            {
                ModelState.AddModelError(" ", "Some problem exists");
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


    }
}
