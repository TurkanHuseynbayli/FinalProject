using BackEnd.DAL;
using BackEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.ViewComponents
{
    public class SubscribeViewComponent:ViewComponent
    {
        private readonly AppDbContext _context;
        public SubscribeViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            SubscribedEmail subscribe = new SubscribedEmail()
            {
                
            };
            return View(await Task.FromResult(subscribe));
        }

    }
}
