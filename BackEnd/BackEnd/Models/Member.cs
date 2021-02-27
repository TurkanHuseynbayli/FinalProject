using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Member
    {
        public int Id { get; set; }
        [Required]
        public string Image { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Specialty { get; set; }
        [NotMapped, Required]
        public IFormFile Photo { get; set; }
        [NotMapped, Required]
        public IFormFile[] Photos { get; set; }
    }
}
