using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<BlogComment> BlogComments { get; set; }
        public List<ProductComment> ProductComments { get; set; }
    }
}
