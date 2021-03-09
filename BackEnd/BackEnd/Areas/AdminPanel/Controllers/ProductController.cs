using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BackEnd.DAL;
using BackEnd.Extensions;
using BackEnd.Helpers;
using BackEnd.Models;
using BackEnd.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BackEnd.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        public ProductController(AppDbContext context, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            _env = env;
            _configuration = configuration;
        }
        public IActionResult Index(int? page = 1)
        {
          
            ViewBag.PageCount = Decimal.Ceiling((decimal)_context.Products
              .Where(pro => pro.IsDeleted == false).Count() / 9);
            ViewBag.Page = page;
            List<Product> products = _context.Products.Where(cr => cr.IsDeleted == false)
                 .Include(cr => cr.ProductDetail).Include(cr => cr.CategoryProducts).ThenInclude(cr => cr.Category)
                 .OrderByDescending(cr => cr.Date).OrderByDescending(pro => pro.Id).Skip(((int)page - 1) * 3).Take(5).ToList();
            return View(products);
        }
      
        public IActionResult Create()
        {
            ViewBag.Categ = _context.Categories.ToList();
            ViewBag.Lists = _context.TabLists.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product products, List<int> CategId, List<int> ListsId)
        {
            ViewBag.Categ = _context.Categories.ToList();
            ViewBag.Lists = _context.TabLists.ToList();
            Product newProduct = new Product();
            ProductDetail newProductDetail = new ProductDetail();


            if (products.Name == null)
            {
                ModelState.AddModelError("Name", "Name cannot be empty");
                return View();
            }

            if (products.Photo == null)
            {
                ModelState.AddModelError("Photo", "Image cannot be empty");
                return View();
            }




            if (!products.Photo.IsImage())
            {
                ModelState.AddModelError("Photos", $"{products.Photo.FileName} - not image type");
                return View(newProduct);
            }

            string folder = Path.Combine("img", "product");
            string fileName = await products.Photo.SaveImageAsync(_env.WebRootPath, folder);
            if (fileName == null)
            {
                return Content("Error");
            }

            newProduct.Image = fileName;
            newProduct.Name = products.Name;
            newProduct.Price = products.Price;
            newProduct.Date = products.Date;
            await _context.AddAsync(newProduct);
            await _context.SaveChangesAsync();


            newProductDetail.Description = products.ProductDetail.Description;

            List<CategoryProduct> categoryProducts = new List<CategoryProduct>();

            if (CategId.Count == 0)
            {
                ModelState.AddModelError("", "Category cannot be empty");
                return View();
            }
           

            foreach (var item in CategId)
            {
                CategoryProduct categoryProduct = new CategoryProduct()
                {
                    ProductId = newProduct.Id,
                    CategoryId = item
                };
                categoryProducts.Add(categoryProduct);
            }
            List<TablistProduct> tablistProducts = new List<TablistProduct>();

            if (ListsId.Count == 0)
            {
                ModelState.AddModelError("", "List cannot be empty");
                return View();
            }

            foreach (var item in ListsId)
            {
                TablistProduct tablistProduct = new TablistProduct()
                {
                    ProductId = newProduct.Id,
                    TabListId = item
                };
                tablistProducts.Add(tablistProduct);
            }
            

            //SEND EMAIL 

            List<SubscribedEmail> emails = _context.SubscribedEmails.Where(e => e.HasDeleted == false).ToList();
            foreach (SubscribedEmail email in emails)
            {
                SendEmail(email.Email, "Yeni bir product yaradildi.", "<h1>Yeni bir product yaradildi</h1>");
            }

            newProduct.CategoryProducts = categoryProducts;
            newProductDetail.ProductId = newProduct.Id;
            await _context.AddAsync(newProductDetail);
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Detail(int? id)
        {
            Product product = await _context.Products.Include(blg => blg.ProductDetail).FirstOrDefaultAsync(blg => blg.Id == id);
            return View(product);
        }

        #region Update
        public IActionResult Update(int? id)
        {
            ViewBag.Categ = _context.Categories.ToList();
            ViewBag.Lists = _context.TabLists.ToList();

            Product products = _context.Products.Include(blg => blg.ProductDetail).FirstOrDefault(blg => blg.Id == id);
            return View(products);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product)
        {
            ViewBag.Categ = _context.Categories.ToList();
            ViewBag.Lists = _context.TabLists.ToList();
            if (id == null) return NotFound();


            Product productOld = await _context.Products.Include(c => c.ProductDetail).FirstOrDefaultAsync(c => c.Id == id);
            Product isExist = _context.Products.Where(cr => cr.IsDeleted == false).FirstOrDefault(cr => cr.Id == id);
            bool exist = _context.Products.Where(cr => cr.IsDeleted == false).Any(cr => cr.Name == product.Name);

            if (exist)
            {
                if (isExist.Name != product.Name)
                {
                    ModelState.AddModelError("Name", "This name already has. Please write another name");
                    return View(productOld);
                }
            }

            if (product == null) return Content("Null");
            if (product.Photo != null)
            {
                if (!product.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", $"{product.Photo.FileName} - not image type");
                    return View(productOld);
                }

                string folder = Path.Combine("img", "product");
                string fileName = await product.Photo.SaveImageAsync(_env.WebRootPath, folder);
                if (fileName == null)
                {
                    return Content("Error");
                }

                Helper.DeleteImage(_env.WebRootPath, folder, productOld.Image);
                productOld.Image = fileName;
            }


            #region Update line
            productOld.Name = product.Name;
            productOld.Price = product.Price;

            productOld.ProductDetail.Description = product.ProductDetail.Description;
            #endregion

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Product product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            int count = _context.Products.Count();
            if (count == 1)
            {
                return Content("sile bilmezsiz");
            }
            return View(product);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {

            if (id == null) return NotFound();
            Product product = _context.Products.FirstOrDefault(c => c.Id == id);
            if (product == null) return NotFound();
            int count = _context.Products.Count();
            if (count == 1)
            {
                return Content("sile bilmezsiz");
            }
            bool isDeleted = Helper.DeleteImage(_env.WebRootPath, "img", product.Image);
            if (!isDeleted)
            {
                ModelState.AddModelError(" ", "Some problem exists");
              
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }



       

        public void SendEmail(string email, string subject, string htmlMessage)
        {
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential()
                {
                    UserName = "turkanhuseynbayli@gmail.com",
                    Password = "12345@Tt"
                }
            };
            MailAddress fromEmail = new MailAddress("turkanhuseynbayli@gmail.com", "Turkan Huseynbayli");
            MailAddress toEmail = new MailAddress(email, "Turkan Huseynbayli");
            MailMessage message = new MailMessage()
            {
                From = fromEmail,
                Subject = subject,
                Body = htmlMessage
            };
            message.To.Add(toEmail);
            client.Send(message);
        }
    }
}
