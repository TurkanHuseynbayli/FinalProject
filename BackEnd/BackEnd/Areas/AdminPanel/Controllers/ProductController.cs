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
using Microsoft.Extensions.Configuration;

namespace BackEnd.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        public ProductController(AppDbContext context, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            _env = env;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            List<Product> products = _context.Products.Where(cr => cr.IsDeleted == false)
                 .Include(cr => cr.ProductDetail).Include(cr => cr.CategoryProducts).ThenInclude(cr => cr.Category)
                 .OrderByDescending(cr => cr.Date).OrderByDescending(pro => pro.Id).Take(10).ToList();
            return View(products);
        }

        #region Create
        public IActionResult Create()
        {
            ViewBag.Categ = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<int> CategId)
        {
            ViewBag.Categ = _context.Categories.ToList();
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool isExist = _context.Products.Where(cr => cr.IsDeleted == false)
                .Any(cr => cr.Name == product.Name);

            if (isExist)
            {
                ModelState.AddModelError("Product.Name", "This name already exist");
                return View();
            }

            Product newProduct = new Product
            {
                Name = product.Name,
            };

            ProductDetail newProductDetail = new ProductDetail();

            #region Images
            if (!product.Photo.IsImage())
            {
                ModelState.AddModelError("Photos", $"{course.Photo.FileName} - not image type");
                return View(newCourse);
            }

            string folder = Path.Combine("img", "course");
            string fileName = await course.Photo.SaveImageAsync(_env.WebRootPath, folder);
            if (fileName == null)
            {
                return Content("Error");
            }
            newCourse.Image = fileName;
            #endregion

            #region Many to Many
            List<CategoryCourse> categoryCourses = new List<CategoryCourse>();
            List<TagCourse> tagCourses = new List<TagCourse>();

            if (CategId.Count == 0)
            {
                ModelState.AddModelError("", "Category cannot be empty");
                return View();
            }

            foreach (var item in CategId)
            {
                CategoryCourse categoryCourse = new CategoryCourse()
                {
                    CourseId = newCourse.Id,
                    CategoriesId = item
                };
                categoryCourses.Add(categoryCourse);
            }

            if (TagId.Count == 0)
            {
                ModelState.AddModelError("", "Tag cannot be empty");
                return View();
            }

            foreach (var item in TagId)
            {
                TagCourse tagCourse = new TagCourse()
                {
                    CourseId = newCourse.Id,
                    TagsId = item
                };
                tagCourses.Add(tagCourse);
            }
            #endregion
            #region Courses
            newCourse.CategoryCourses = categoryCourses;
            newCourse.TagCourses = tagCourses;
            course.CreatedTime = DateTime.Now;
            newCourse.CreatedTime = course.CreatedTime;
            await _context.Courses.AddAsync(newCourse);
            await _context.SaveChangesAsync();
            #endregion
            #region CourseDetail
            newCourseDetail.AboutCourseDescription = course.CourseDetail.AboutCourseDescription;
            newCourseDetail.HowToApplyExplaining = course.CourseDetail.HowToApplyExplaining;
            newCourseDetail.CertificationExplain = course.CourseDetail.CertificationExplain;
            newCourseDetail.Starts = course.CourseDetail.Starts;
            newCourseDetail.Duration = course.CourseDetail.Duration;
            newCourseDetail.ClassDuration = course.CourseDetail.ClassDuration;
            newCourseDetail.SkillLevel = course.CourseDetail.SkillLevel;
            newCourseDetail.Language = course.CourseDetail.Language;
            newCourseDetail.StudentsCount = course.CourseDetail.StudentsCount;
            newCourseDetail.StudentsCount = course.CourseDetail.StudentsCount;
            newCourseDetail.Assesments = course.CourseDetail.Assesments;
            newCourseDetail.CoursePrice = course.CourseDetail.CoursePrice;
            newCourseDetail.CourseId = newCourse.Id;
            await _context.AddAsync(newCourseDetail);
            await _context.SaveChangesAsync();
            #endregion

           

            //return Json(newCourse);
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
