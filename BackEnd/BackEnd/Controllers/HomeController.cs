using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BackEnd.Models;
using BackEnd.DAL;
using BackEnd.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BackEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Index()
        {
         
            HomeVM homeVM = new HomeVM
            {

                Sliders = _context.Sliders.ToList(),
                Banners = _context.Banners.FirstOrDefault(),
                Blogs = _context.Blogs.Take(4).ToList(),
                BlogDetails = _context.BlogDetails.ToList(),
                Categories = _context.Categories.ToList(),
                CategoryBlogs = _context.CategoryBlogs.ToList(),
                CategoryProducts = _context.CategoryProducts.ToList(),
                Contacts = _context.Contacts.FirstOrDefault(),
                Discounts = _context.Discounts.ToList(),
                Products = _context.Products.Include(tb => tb.TablistProduct).ThenInclude(tb => tb.TabList).Take(10).ToList(),
                TabLists = _context.TabLists.Include(tb => tb.TablistProduct).ThenInclude(t=>t.Product).ToList(),

            };
            return View(homeVM);
        }
       


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
