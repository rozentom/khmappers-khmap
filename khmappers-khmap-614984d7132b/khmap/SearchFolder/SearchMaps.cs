using khmap.DataBaseProviders;
using khmap.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace khmap.SearchFolder
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

        public object search(string text)
        {
            return searchMapsFunc(text);
        }

        public IEnumerable<Map> searchMapsFunc(string mapName)
        {
            List<Map> maps = new List<Map>();
            IEnumerable<Map> mapss = _allMaps;
            foreach (var item in mapss)
            {
                if (item.Name.ToLower().Contains(mapName.ToLower()))
                {
                    maps.Add(item);
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