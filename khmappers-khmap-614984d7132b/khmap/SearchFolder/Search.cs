using khmap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace khmap.SearchFolder
{
    public class Search
    {
        private ISearchGroups _isg;
        private ISearchMaps  _ism;
        private ISearchUsers _isu;

        public Search()
        {
            _isg = new SearchGroups();
            _ism = new SearchMaps();
            _isu = new SearchUsers();
        }

        public ISearchMaps Ism
        {
            get
            {
                return _ism;
            }

            set
            {
                _ism = value;
            }
        }
        public ISearchUsers Isu
        {
            get
            {
                return _isu;
            }

            set
            {
                _isu = value;
            }
        }
        internal ISearchGroups Isg
        {
            get
            {
                return _isg;
            }

            set
            {
                _isg = value;
            }
        }

        public IEnumerable<Group> searchGroups(string text)
        {
            return _isg.SearchGroupsFunc(text);
        }
        public IEnumerable<ApplicationUser> searchUsers(string text)
        {
            return _isu.SearchUsersFunc(text);
        }

        public IEnumerable<Map> searchMaps(string text)
        {
            return _ism.searchMapsFunc(text);
        }

    }
}