using khmap.DataBaseProviders;
using khmap.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace khmap.SearchDirectory
{
    public class SearchMaps : ISearchMaps, ISearchType
    {
        IEnumerable<Map> _allMaps;
        private MapDB _mapManager;

        public SearchMaps()
        {
            _mapManager = new MapDB(new Settings());
            _allMaps = _mapManager.GetAllMaps();
        }

        public bool sameType(ISearchType st)
        {
            if(st is SearchMaps)
            {
                return true;
            }
            return false;
        }

        public object search(string text, ObjectId userId)
        {
            return searchMapsFunc(text, userId);
        }

        public IEnumerable<Map> searchMapsFunc(string mapName, ObjectId userId)
        {
            List<Map> maps = new List<Map>();
            IEnumerable<Map> mapss = _allMaps;
            
            foreach (var item in mapss)
            {
                if (item.Name.ToLower().Contains(mapName.ToLower()))
                {
                    if (item.Permissions.Users.Keys.Contains(userId))
                    {
                        maps.Add(item);
                    }
                    else
                    {

                        var allMapGroupsId = item.Permissions.Groups.Keys;
                        var allUserGroups = new GroupDB(new Settings()).GetAllGroupsOfUser(userId);
                        foreach (var group in allUserGroups)
                        {
                            if (allMapGroupsId.Contains(group.Id))
                            {
                                maps.Add(item);
                            }
                        }
                    }
                }
                var nodes = item.Model["nodeDataArray"];
                foreach (var node in nodes.AsBsonArray)
                {
                    if (node["text"].ToString().Contains(mapName.ToLower()) && !maps.Contains(item))
                    {
                        maps.Add(item);
                    }
                }
            }
            return maps;
        }
    }
}