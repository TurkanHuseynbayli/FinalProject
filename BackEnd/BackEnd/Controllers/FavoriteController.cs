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
    public class FavoriteController : Controller
    {
        private readonly AppDbContext _context;
        public FavoriteController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<FavoriteVM> dbBasket = new List<FavoriteVM>();
            ViewBag.Total = 0;
            if (Request.Cookies["favorite"] != null)
            {

                List<FavoriteVM> favorite = JsonConvert.DeserializeObject<List<FavoriteVM>>(Request.Cookies["favorite"]);

                foreach (FavoriteVM pro in favorite)
                {
                    Product dbProduct = await _context.Products.FindAsync(pro.Id);
                    pro.Price = dbProduct.Price;
                    pro.Image = dbProduct.Image;
                    pro.New = dbProduct.New;
                    dbBasket.Add(pro);
                    ViewBag.Total += pro.Price;
                }
            }



            return View(dbBasket);
        }
        public async Task<IActionResult> AddFavorite(int? id)
        {
            if (id == 0) return NotFound();
            Product product = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);
            if (product == null) return NotFound();
            List<FavoriteVM> favorite;
            if (Request.Cookies["favorite"] != null)
            {
                favorite = JsonConvert.DeserializeObject<List<FavoriteVM>>(Request.Cookies["favorite"]);
            }
            else
            {
                favorite = new List<FavoriteVM>();
            }
            FavoriteVM isExist = favorite.FirstOrDefault(p => p.Id == id);
            if (isExist == null)
            {
                favorite.Add(new FavoriteVM
                {
                    Id = (int)id,
                    Count = 1
                });
            }
            else
            {
                isExist.Count += 1;
            }

            Response.Cookies.Append("favorite", JsonConvert.SerializeObject(favorite));
            return RedirectToAction(nameof(Index));

        }

       
    }
}
