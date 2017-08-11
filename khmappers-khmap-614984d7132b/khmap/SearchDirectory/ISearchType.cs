using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace khmap.SearchDirectory
{
    public interface ISearchType
    {
        bool sameType(ISearchType st);

        Object search(string text, ObjectId userId);
    }
}
