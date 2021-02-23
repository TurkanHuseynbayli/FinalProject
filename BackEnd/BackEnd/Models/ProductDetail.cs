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
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string Information { get; set; }
        public string Tags { get; set; }
        [Required]
        public string Rate { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
