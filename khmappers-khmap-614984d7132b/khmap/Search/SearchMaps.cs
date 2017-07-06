using khmap.DataBaseProviders;
using khmap.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2
{
    public class SearchMaps : ISearchMaps
    {
        IEnumerable<Map> _allMaps;
        private MapDB _mapManager;

        public SearchMaps()
        {
            _mapManager = new MapDB(new Settings());
            _allMaps = _mapManager.GetAllMaps();
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