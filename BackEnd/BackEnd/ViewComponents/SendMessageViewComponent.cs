using BackEnd.DAL;
using BackEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.ViewComponents
{
    public class SendMessageViewComponent:ViewComponent
    {
        private readonly AppDbContext _context;
        public SendMessageViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            CommentVM comment = new CommentVM();
            return View(await Task.FromResult(comment));
        }
    }
}
