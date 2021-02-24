using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly AppDbContext _context;
        public FavoriteController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Products.Include(pro => pro.ProductDetail).Take(9).ToList());
        }
    }
}
