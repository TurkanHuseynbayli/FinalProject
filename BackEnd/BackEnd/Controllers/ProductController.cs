using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DAL;
using BackEnd.Models;
using BackEnd.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        
        public ProductController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index(int? page = 1)
        {
            ViewBag.PageCount = Decimal.Ceiling((decimal)_context.Products
               .Where(pro => pro.IsDeleted == false).Count() / 9);
            ViewBag.Page = page;

            ProductVM productVM = new ProductVM
            {


                Categories = _context.Categories.ToList(),
                Products = _context.Products.Include(pro => pro.ProductDetail).Skip(((int)page-1)*3).Take(6).ToList(),
            };

            return View(productVM);
        }

        public IActionResult FilterCategory(int? id)
        {

            if (id == null) return NotFound();
            List<CategoryProduct> categoryProducts = _context.CategoryProducts.Include(c => c.Product)
                .Where(c => c.CategoryId == id).ToList();
            if (categoryProducts == null) return RedirectToAction("Index", "Error");
            List<Product> products = categoryProducts.Select(x => x.Product).Where(c => c.IsDeleted == false).ToList();
            if (products == null) return NotFound();

            return View("~/Views/Shared/Components/Product/Default2.cshtml", products);
        }
        
        public IActionResult Detail(int? id)
        {
            if (id == null) return NotFound();
            TempData["BlogId"] = (int)id;
            ProductVM productVM = new ProductVM()
            {
                Products= _context.Products.Include(pro => pro.ProductDetail).OrderByDescending(pro => pro.Id).Take(4).ToList(),
                Categories = _context.Categories.ToList(),
                Product= _context.Products.Include(pro => pro.ProductDetail).FirstOrDefault(pro => pro.Id == id),
                ProductComments = _context.ProductComments.Where(b => b.ProductId == id).Include(b => b.Comment).ToList(),
            };
            return View(productVM);
        }

        public IActionResult ProductSearch(string search)
        {

            List<Product> product = _context.Products.Where(pro => pro.Name.Contains(search) && pro.IsDeleted ==false).ToList();

            return PartialView("_ProductSearchPartial", product);
        }

        public async Task<IActionResult> AddComment(string name, string surname, string review, int? id)
        {

            if (id == null) return NotFound();
            if (review == null)
            {
                ModelState.AddModelError("", "Text cannot be empty");
                return RedirectToAction("Detail", "Product", new { id });
            }
            ProductComment productComment = new ProductComment();
            Comment comment = new Comment();
            if (!User.Identity.IsAuthenticated)
            {
                if (name == null)
                {
                    ModelState.AddModelError("", "Name cannot be empty");
                    return RedirectToAction("Detail", "Product", new { id });
                }

                if (surname == null)
                {
                    ModelState.AddModelError("", "Surname cannot be empty");
                    return RedirectToAction("Detail", "Product", new { id });
                }

                comment.Name = name;
                comment.Surname = surname;

            }
            else
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                comment.Name = user.Name;
                comment.Surname = user.Surname;

            }
            comment.Text = review;
            comment.TimeStamp = DateTime.Now;
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            productComment.CommentId = comment.Id;
            productComment.ProductId = (int)id;
            await _context.ProductComments.AddAsync(productComment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", "Product", new { id });
        }
    }
}
