using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Bio
    {
        public int Id { get; set; }
        [Required]
        public string Logo { get; set; }
        public string Address { get; set; }
        public string Phono { get; set; }
        public string Email { get; set; }
    }
}
