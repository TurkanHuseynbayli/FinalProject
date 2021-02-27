using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Contact
    {
        public int Id { get; set; }
        [Required]
        public string Image { get; set; }

        [Required]

        public string Email { get; set; }
        public string Phone { get; set; }
  
        [NotMapped]
        public IFormFile Photo { get; set; }

    }
}
