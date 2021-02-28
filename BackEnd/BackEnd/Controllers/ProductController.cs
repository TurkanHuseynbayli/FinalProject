using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DAL;
using BackEnd.Models;
using BackEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
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
            ProductVM productVM = new ProductVM()
            {
                Products= _context.Products.Include(pro => pro.ProductDetail).OrderByDescending(pro => pro.Id).Take(4).ToList(),
                Categories = _context.Categories.ToList(),
                Product= _context.Products.Include(pro => pro.ProductDetail).FirstOrDefault(pro => pro.Id == id)

            };
            return View(productVM);
        }

        public IActionResult ProductSearch(string search)
        {

            List<Product> product = _context.Products.Where(pro => pro.IsDeleted == false).Where(pro => pro.Name.Contains(search)).ToList();

            return PartialView("_ProductSearchPartial", product);
        }
    }
}
