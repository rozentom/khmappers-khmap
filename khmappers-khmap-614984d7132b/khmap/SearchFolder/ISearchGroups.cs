using khmap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace khmap.SearchFolder
{
    interface ISearchGroups
    {
        IEnumerable<Group> SearchGroupsFunc(string groupName);
    }
}
