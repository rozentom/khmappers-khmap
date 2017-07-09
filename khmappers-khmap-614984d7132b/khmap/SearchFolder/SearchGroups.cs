using khmap.DataBaseProviders;
using khmap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace khmap.SearchFolder
{
    public class SearchGroups : ISearchGroups, ISearchType
    {
        private IEnumerable<Group> _allGroups;
        private GroupDB _groupsManeger;
        public SearchGroups()
        {
            _groupsManeger = new GroupDB(new Settings());
            _allGroups = _groupsManeger.GetAllGroups();
        }

        public bool sameType(ISearchType st)
        {
            if(st is SearchGroups)
            {
                return true;
            }
            return false;
        }

        public object search(string text)
        {
            return SearchGroupsFunc(text);
        }

        public IEnumerable<Group> SearchGroupsFunc(string groupName)
        {
            List<Group> groups = new List<Group>();
            IEnumerable<Group> groupss = _allGroups;
            foreach (var item in groupss)
            {
                if (item.Name.ToLower().Contains(groupName.ToLower()))
                {
                    groups.Add(item);
                }
            }
            return groups;
        }
    }
}