using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class ProductDetail
    {
        public int Id { get; set; }
        [Required]
        public string Image { get; set; }
        public double Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
