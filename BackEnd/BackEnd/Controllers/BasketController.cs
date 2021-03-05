using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DAL;
using BackEnd.Models;
using BackEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BackEnd.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        public BasketController( AppDbContext context)
        {
           
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketVM> dbBasket = new List<BasketVM>();
            ViewBag.Total = 0;
            ViewBag.SinglePrice = 0;
            if (Request.Cookies["basket"] != null)
            {

                List<BasketVM> basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);

                foreach (BasketVM pro in basket)
                {
                    Product dbProduct = await _context.Products.FindAsync(pro.Id);
                    ViewBag.SinglePrice = dbProduct.Price;
                    pro.Price = dbProduct.Price * pro.Count;
                    pro.Image = dbProduct.Image;
                    dbBasket.Add(pro);
                    ViewBag.Total += pro.Price;
                }
            }



            return View(dbBasket);
        }
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == 0) return NotFound();
            Product product = await _context.Products.FirstOrDefaultAsync(c=>c.Id==id);
            if (product == null) return NotFound();
            List<BasketVM> basket;
            if (Request.Cookies["basket"] != null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            }
            else
            {
                basket = new List<BasketVM>();
            }
            BasketVM isExist = basket.FirstOrDefault(p => p.Id == id);
            if (isExist == null)
            {
                basket.Add(new BasketVM
                {
                    Id = (int)id,
                    Count = 1
                });
            }
            else
            {
                isExist.Count += 1;
            }
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
            return RedirectToAction(nameof(Index));

        }

        public IActionResult RemoveCount(int? id)
        {
            List<BasketVM> basket = new List<BasketVM>();

            basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            //BasketVM remove = basket.FirstOrDefault(p => p.Id == id);
            
            BasketVM isExist = basket.FirstOrDefault(p => p.Id == id);
            if (isExist.Count>1)
            {
                isExist.Count--;
                
            }
            else
            {
                basket.Remove(isExist);
            }
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));

            return RedirectToAction(nameof(Index));
        }
        public IActionResult RemoveItem(int? id)
        {
            List<BasketVM> basket = new List<BasketVM>();

            basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            BasketVM remove = basket.FirstOrDefault(p => p.Id == id);
          
            basket.Remove(remove);
           
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));

            return RedirectToAction(nameof(Index));
        }
    }
}
