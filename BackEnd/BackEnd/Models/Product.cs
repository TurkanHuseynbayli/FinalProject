using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public double Price { get; set; }
        public string Name { get; set; }
        [Required, DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        [Required]
        public int Rate { get; set; }
        public ProductDetail ProductDetail { get; set; }
        public ICollection<CategoryProduct> CategoryProducts { get; set; }
        public ICollection<TablistProduct> TablistProduct { get; set; }


    }
}
