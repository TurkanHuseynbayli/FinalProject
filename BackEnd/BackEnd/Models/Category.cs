using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string NameCategory { get; set; }
        public ICollection<CategoryBlog> CategoryBlogs { get; set; }

    }
}
