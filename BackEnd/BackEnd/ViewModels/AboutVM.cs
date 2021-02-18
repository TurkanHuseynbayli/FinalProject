using BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.ViewModels
{
    public class AboutVM
    {
        public About Abouts { get; set; }
        public List<Member> Members { get; set; }
    }
}
