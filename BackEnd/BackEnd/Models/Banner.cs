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
        public string Image { get; set; }
        [Required]
        public string Description { get; set; }
        
   
    }
}
