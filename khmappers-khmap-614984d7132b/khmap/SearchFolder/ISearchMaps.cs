using khmap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace khmap.SearchFolder
{
    public interface ISearchMaps
    {
        IEnumerable<Map> searchMapsFunc(string text);
    }
}
