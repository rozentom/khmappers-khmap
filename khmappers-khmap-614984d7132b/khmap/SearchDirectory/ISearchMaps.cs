using khmap.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace khmap.SearchDirectory
{
    public interface ISearchMaps
    {
        IEnumerable<Map> searchMapsFunc(string text, ObjectId userId);
    }
}
