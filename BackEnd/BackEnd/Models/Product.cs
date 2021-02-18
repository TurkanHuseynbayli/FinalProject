﻿using System;
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
        public double Price { get; set; }
        public string Name { get; set; }
        [Required, DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        public ProductDetail ProductDetail { get; set; }
        public ICollection<CategoryProduct> CategoryProducts { get; set; }

    }
}
