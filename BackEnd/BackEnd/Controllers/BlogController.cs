using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DAL;
using BackEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackEnd.Controllers
{
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;

        private readonly AppDbContext _context;
        public BlogController(ILogger<BlogController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail()
        {
            //if (id == null) return NotFound();
            //HomeVM homeVM = new HomeVM()
            //{


            //    Blogs = _context.Blogs.Include(blg => blg.BlogDetail).ToList()

            //};
            return View();
        }
    }
}
