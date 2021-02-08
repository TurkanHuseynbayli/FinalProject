using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Slider
    {
        public int Id { get; set; }
        [Required]
        public string Image { get; set; }
        [StringLength(150)]
        public string Title { get; set; }
        [StringLength(150)]
        public string SubTitle { get; set; }
    }
}
