using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Blog
    {
        public int Id { get; set; }
        [Required]
        public string Image { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        public string Name { get; set; }
        public int Discount { get; set; }
        public string Description { get; set; }
        [Required, DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        public bool isDelete { get; set; }
        public BlogDetail BlogDetail { get; set; }
        public ICollection<CategoryBlog> CategoryBlogs { get; set; }

    }
}
