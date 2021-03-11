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
using Microsoft.Extensions.Logging;

namespace BackEnd.Controllers
{
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        public BlogController(ILogger<BlogController> logger, AppDbContext context, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int? id)
        {
            if (id == null) return NotFound();
            TempData["BlogId"] = (int)id;
            BlogVM blogVM = new BlogVM()
            {
                Blogs = _context.Blogs.Include(pro => pro.BlogDetail).OrderByDescending(pro => pro.Id).Take(4).ToList(),
                Categories = _context.Categories.ToList(),
                Blog = _context.Blogs.Include(pro => pro.BlogDetail).FirstOrDefault(pro => pro.Id == id),
                BlogComments = _context.BlogComments.Where(b => b.BlogId == id).Include(b => b.Comment).ToList(),

            };
            return View(blogVM);
        }
       
        public async Task<IActionResult> AddComment(string name, string surname, string review ,int? id)
        {

            if (id == null) return NotFound();
            if (review == null)
            {
                ModelState.AddModelError("", "Text cannot be empty");
                return RedirectToAction("Detail", "Blog", new {id});
            }
            BlogComment blogComment = new BlogComment();
            Comment comment = new Comment();
            if (!User.Identity.IsAuthenticated)
            {
                if (name == null)
                {
                    ModelState.AddModelError("", "Name cannot be empty");
                    return RedirectToAction("Detail", "Blog", new { id });
                }

                if (surname == null)
                {
                    ModelState.AddModelError("", "Surname cannot be empty");
                    return RedirectToAction("Detail", "Blog", new { id });
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
            blogComment.CommentId = comment.Id;
            blogComment.BlogId = (int)id;
            await _context.BlogComments.AddAsync(blogComment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", "Blog", new { id });
        }

    }
}
