using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BackEnd.DAL;
using BackEnd.Extensions;
using BackEnd.Models;
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

        #region Create
        public IActionResult Create()
        {
            ViewBag.Categ = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<int> CategId)
        {
            ViewBag.Categ = _context.Categories.ToList();
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool isExist = _context.Products.Where(cr => cr.IsDeleted == false)
                .Any(cr => cr.Name == product.Name);

            if (isExist)
            {
                ModelState.AddModelError("Product.Name", "This name already exist");
                return View();
            }

            Product newProduct = new Product
            {
                Name = product.Name,
            };

            ProductDetail newProductDetail = new ProductDetail();

            #region Images
            if (!product.Photo.IsImage())
            {
                ModelState.AddModelError("Photos", $"{product.Photo.FileName} - not image type");
                return View(newProduct);
            }

            string folder = Path.Combine("img", "product");
            string fileName = await product.Photo.SaveImageAsync(_env.WebRootPath, folder);
            if (fileName == null)
            {
                return Content("Error");
            }
            newProduct.Image = fileName;
            #endregion

            #region Many to Many
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

          

       
            #endregion
            #region Products
            newProduct.CategoryProducts = categoryProducts;
            product.Date = DateTime.Now;
            newProduct.Date = product.Date;
            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();
            #endregion
            #region ProductDetail
            newProductDetail.Description = product.ProductDetail.Description;
          
            await _context.AddAsync(newProductDetail);
            await _context.SaveChangesAsync();
            #endregion

           

       
            return RedirectToAction(nameof(Index));
        }
        #endregion

        //SEND EMAIL 

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
                    UserName = "turkanhuseyinbeyli@gmail.com",
                    Password = "eti228"
                }
            };
            MailAddress fromEmail = new MailAddress("turkanhuseyinbeyli@gmail.com", "Turkan Huseynbayli");
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
