using BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.ViewModels
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public Banner Banners { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<BlogDetail> BlogDetails { get; set; }
        public List<Category> Categories { get; set; }
        public List<CategoryBlog> CategoryBlogs { get; set; }
        public List<CategoryProduct> CategoryProducts { get; set; }
        public Contact Contacts { get; set; }
        public List<Discount> Discounts { get; set; }
        public List<Product> Products { get; set; }
        public Product Product { get; set; }
        public List<RecentProduct> RecentProducts { get; set; }
       
    }
}
