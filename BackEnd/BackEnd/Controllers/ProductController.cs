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
        public IActionResult Index()
        {
            ProductVM productVM = new ProductVM
            {
                Categories = _context.Categories.ToList(),
                Products = _context.Products.Include(cg => cg.ProductDetail).ToList(),
            };

            return View(productVM);
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
    }
}
