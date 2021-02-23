using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class About
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; }

    }
}
