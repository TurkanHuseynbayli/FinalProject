using BackEnd.DAL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.ViewComponents
{
    public class BlogViewComponent:ViewComponent
    {
        private readonly AppDbContext _context;
        public BlogViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? take)
        {
            return View(_context.Blogs.Take((int)take).ToList());
        }
    }
}
