using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Banner
    {
        public int Id { get; set; }
        [Required]
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public double Price { get; set; }
        public string ProductName { get; set; }
   
    }
}
