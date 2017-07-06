using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using khmap.Models;

namespace khmap.DataBaseProviders
{
    public class MapDB
    {
        private readonly Settings _settings;
        private readonly MongoDatabase _database;
        private readonly string _collectionName;

        public MapDB(Settings settings)
        {
            _settings = settings;
            _collectionName = _settings.MapsCollectionName;
            _database = Connect();
        }

        public string AddMap(Map map)
        {
            _database.GetCollection<Map>(_collectionName).Save(map);
            return map.Id.ToString();
        }

        public IEnumerable<Map> GetAllMaps()
        {
            var maps = _database.GetCollection<Map>(_collectionName).FindAll();
            return maps;
        }

        public Map GetMapById(ObjectId id)
        {
            var query = Query<Map>.EQ(e => e.Id, id);
            var map = _database.GetCollection<Map>(_collectionName).FindOne(query);
            return map;
        }

        public IEnumerable<Map> GetMapsByCreatorId(ObjectId creatorId)
        {
            var query = Query<Map>.EQ(e => e.Creator, creatorId);
            var maps = _database.GetCollection<Map>(_collectionName).Find(query);
            return maps;
        }


        public IEnumerable<Map> GetSharedMapsById(ObjectId userId)
        {
            var maps = _database.GetCollection<Map>(_collectionName).FindAll();
            var res = maps.Where(x => x.Permissions.Users.Keys.Contains(userId, new IDComparer()));
            res = res.Where(x => x.Creator != userId);
            return res;
        }


        public bool IsMapExist(ObjectId mapId)
        {
            var map = GetMapById(mapId);
            return map != null;
        }

        public bool RemoveMap(ObjectId id)
        {
            var query = Query<Map>.EQ(e => e.Id, id);
            var result = _database.GetCollection<Map>(_collectionName).Remove(query);

            return GetMapById(id) == null;
        }

        public void UpdateMap(Map map)
        {
            var query = Query<Map>.EQ(e => e.Id, map.Id);
            var update = Update<Map>.Replace(map); // update modifiers
            _database.GetCollection<Map>(_collectionName).Update(query, update);
        }

        public IEnumerable<Map> SearchMap(string mapName)
        {
            List<Map> maps = new List<Map>();
            IEnumerable<Map> mapss = _database.GetCollection<Map>(_collectionName).FindAll();
            foreach (var item in mapss)
            {
                if (item.Name.ToLower().Contains(mapName.ToLower()))
                {
                    maps.Add(item);
                }
                var nodes = item.Model["nodeDataArray"];
                foreach(var node in nodes.AsBsonArray)
                {
                    if (node["text"].ToString().Contains(mapName.ToLower())  && !maps.Contains(item))
                    {
                        maps.Add(item);
                    }
                }
            }
            return maps;
        }

        //todo
        public IEnumerable<Map> GetAllMapContainsGroups(IEnumerable<Group> groups)
        {
            var maps = _database.GetCollection<Map>(_collectionName).FindAll();
            List<Map> mapsList = new List<Map>();
            foreach (var group in groups)
            {
                var tmpMapsList = maps.Where(x => x.Permissions.Groups.Keys.Contains(group.Id, new IDComparer()));
                mapsList.AddRange(tmpMapsList);
            }
            var res = mapsList.Distinct(new MapComparer());
            return res;
        }

        public IEnumerable<Map> GetAllMapContainsGroupsNotOwned(string userId, IEnumerable<Group> groups)
        {
            var maps = GetAllMapContainsGroups(groups);
            maps = maps.Where(x => x.Creator != (new ObjectId(userId)));
            return maps;
        }

        public bool IsMapOwner(string mapId, string userId)
        {
            var map = GetMapById(new ObjectId(mapId));
            return map.Creator.ToString().Equals(userId);
        }

        private MongoDatabase Connect()
        {
            var client = new MongoClient(_settings.ConnectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(_settings.DatabaseName);
            return database;
        }
    }
}