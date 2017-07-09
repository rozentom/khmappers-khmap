using khmap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace khmap.SearchFolder
{
    public class Search : ISearch
    {
        private ISearchGroups _isg;
        private ISearchMaps  _ism;
        private ISearchUsers _isu;
        private List<ISearchType> _searchTypes;

        public Search()
        {
            _isg = new SearchGroups();
            _ism = new SearchMaps();
            _isu = new SearchUsers();
            _searchTypes = new List<ISearchType>();
            _searchTypes.Add((ISearchType)_isg);
            _searchTypes.Add((ISearchType)_ism);
            _searchTypes.Add((ISearchType)_isu);
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

        //search maps, users, groups by text and adding them to 1 dictionary.
        //includes only searches that were in typesNoIncluded list.
        //return a dictionary where each node has object of one of the searches and the results
        public Dictionary<ISearchType, Object> searchFunc(string text, List<ISearchType> typesIncluded)
        {
            Boolean searchThisType = false;
            Dictionary<ISearchType, Object> searchResults = new Dictionary<ISearchType, object>();
            foreach(ISearchType st in _searchTypes)
            {
                foreach(ISearchType st_notIncluded in typesIncluded)
                {
                    if (st.sameType(st_notIncluded))
                    {
                        searchThisType = true;
                    }
                }
                if (searchThisType)
                {
                    var result = st.search(text);
                    searchResults.Add(st, result);
                }
                searchThisType = false;
            }
            return searchResults;
        }

        public IEnumerable<Group> getGroupsOfResult(Dictionary<ISearchType, object> results)
        {
            foreach (ISearchType key in results.Keys)
            {
                if(key is SearchGroups)
                {
                    return (IEnumerable<Group>)results[key];
                }
            }
            return null;
        }

        public IEnumerable<Map> getMapsOfResult(Dictionary<ISearchType, object> results)
        {
            foreach (ISearchType key in results.Keys)
            {
                if (key is SearchMaps)
                {
                    return (IEnumerable<Map>)results[key];
                }
            }
            return null;
        }

        public IEnumerable<ApplicationUser> getUsersOfResult(Dictionary<ISearchType, object> results)
        {
            foreach (ISearchType key in results.Keys)
            {
                if (key is SearchUsers)
                {
                    return (IEnumerable<ApplicationUser>)results[key];
                }
            }
            return null;
        }
    }
}