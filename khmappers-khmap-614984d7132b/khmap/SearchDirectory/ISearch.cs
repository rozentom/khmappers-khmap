using khmap.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace khmap.SearchDirectory
{
    public interface ISearch
    {
        IEnumerable<Map> searchMaps(string text, ObjectId userId);

        IEnumerable<ApplicationUser> searchUsers(string text, ObjectId userId);

        IEnumerable<Group> searchGroups(string text, ObjectId userId);

        Dictionary<ISearchType, Object> searchFunc(string text, ObjectId userId, List<ISearchType> typesIncluded);

        IEnumerable<Group> getGroupsOfResult(Dictionary<ISearchType, Object> results);
        IEnumerable<Map> getMapsOfResult(Dictionary<ISearchType, Object> results);
        IEnumerable<ApplicationUser> getUsersOfResult(Dictionary<ISearchType, Object> results);



    }
}
