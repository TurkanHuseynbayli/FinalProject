using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class ErrorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ErrorController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
       
        public IActionResult Index()
        {
            return View();
        }
    }
}
