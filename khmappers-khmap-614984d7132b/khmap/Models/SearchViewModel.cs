using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace khmap.Models
{
    public class SearchViewModel
    {
        public IEnumerable<Map> Maps { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public string SearchText { get; set; }
    }
}