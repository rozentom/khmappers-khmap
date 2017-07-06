using khmap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication2
{
    public interface ISearchMaps
    {
        IEnumerable<Map> searchMapsFunc(string text);
    }
}
