﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class CategoryBlog
    {
        public int Id { get; set; }
        public virtual Category Category { get; set; }
        public int CategoryId { get; set; }
        public virtual Blog Blog { get; set; }
        public int BlogId { get; set; }
    }
}
