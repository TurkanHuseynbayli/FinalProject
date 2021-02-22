﻿using System;
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
        public async Task<IActionResult> Basket()
        {
            List<BasketVM> dbBasket = new List<BasketVM>();
            ViewBag.Total = 0;
            if (Request.Cookies["basket"] != null)
            {

                List<BasketVM> basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);

                foreach (BasketVM pro in basket)
                {
                    Product dbProduct = await _context.Products.FindAsync(pro.Id);
                    pro.Price = dbProduct.Price * pro.Count;
                    pro.Image = dbProduct.Image;
                    dbBasket.Add(pro);
                    ViewBag.Total += pro.Price;
                }
            }



            return View(dbBasket);
        }
        public async Task<IActionResult> AddBasket(int id)
        {
            Product product = await _context.Products.FindAsync(id);
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
                    Id = id,
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

        public IActionResult RemoveItem(int? id)
        {
            List<BasketVM> basket = new List<BasketVM>();

            basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            BasketVM remove = basket.FirstOrDefault(p => p.Id == id);
            basket.Remove(remove);
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));

            return RedirectToAction(nameof(Basket));
        }
    }
}
