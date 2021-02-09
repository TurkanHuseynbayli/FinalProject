﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Discount
    {
        public int Id { get; set; }
        [Required]
        public string Image { get; set; }
        public string Description { get; set; }
      
    }
}
