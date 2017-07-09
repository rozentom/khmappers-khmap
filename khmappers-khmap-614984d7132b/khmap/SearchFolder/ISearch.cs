using khmap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace khmap.SearchFolder
{
    public interface ISearch
    {
        IEnumerable<Map> searchMaps(string text);

        IEnumerable<ApplicationUser> searchUsers(string text);

        IEnumerable<Group> searchGroups(string text);

        Dictionary<ISearchType, Object> searchFunc(string text, List<ISearchType> typesIncluded);

        IEnumerable<Group> getGroupsOfResult(Dictionary<ISearchType, Object> results);
        IEnumerable<Map> getMapsOfResult(Dictionary<ISearchType, Object> results);
        IEnumerable<ApplicationUser> getUsersOfResult(Dictionary<ISearchType, Object> results);



    }
}
