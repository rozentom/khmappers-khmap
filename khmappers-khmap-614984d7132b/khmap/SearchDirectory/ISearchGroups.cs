using khmap.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace khmap.SearchDirectory
{
    interface ISearchGroups
    {
        IEnumerable<Group> SearchGroupsFunc(string groupName, ObjectId userId);
    }
}
