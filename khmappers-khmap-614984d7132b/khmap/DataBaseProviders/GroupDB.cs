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
    public class GroupDB
    {
        private readonly Settings _settings;
        private readonly MongoDatabase _database;
        private readonly string _collectionName;

        public GroupDB(Settings settings)
        {
            _settings = settings;
            _collectionName = _settings.GroupsCollectionName;
            _database = Connect();
        }

        public void AddGroup(Group group)
        {
            _database.GetCollection<Group>(_collectionName).Save(group);
        }

        public IEnumerable<Group> GetAllGroups()
        {
            var groups = _database.GetCollection<Group>(_collectionName).FindAll();
            return groups;
        }

        public IEnumerable<Group> GetAllOwnedGroupsOfUser(ObjectId userId)
        {
            var query = Query<Group>.EQ(e => e.Creator, userId);
            var groups = _database.GetCollection<Group>(_collectionName).Find(query);
            return groups;
        }

        public IEnumerable<Group> GetAllMemberGroupsOfUser(ObjectId userId)
        {
            var groups = _database.GetCollection<Group>(_collectionName).FindAll();
            var res = groups.Where(x => x.Members.Keys.Contains(userId, new IDComparer()));
            res = res.Where(x => x.Members[userId] != GroupPermissionType.Owner);
            return res;
        }

        public IEnumerable<Group> GetAllGroupsOfUser(ObjectId userId)
        {
            var groups = _database.GetCollection<Group>(_collectionName).FindAll();
            var res = groups.Where(x => x.Members.Keys.Contains(userId, new IDComparer()));
            return res;
        }

        public Group GetGroupById(ObjectId id)
        {
            var query = Query<Group>.EQ(e => e.Id, id);
            var group = _database.GetCollection<Group>(_collectionName).FindOne(query);
            return group;
        }

        public bool IsGroupExist(ObjectId id)
        {
            var group = GetGroupById(id);
            return group != null;
        }

        public bool RemoveGroup(ObjectId id)
        {
            var query = Query<Group>.EQ(e => e.Id, id);
            var result = _database.GetCollection<Group>(_collectionName).Remove(query);

            return GetGroupById(id) == null;
        }

        public void UpdateGroup(Group group)
        {
            var query = Query<Group>.EQ(e => e.Id, group.Id);
            var update = Update<Group>.Replace(group); // update modifiers
            _database.GetCollection<Group>(_collectionName).Update(query, update);
        }

        public ObjectId GetGroupCreatorId(ObjectId groupId)
        {
            var query = Query<Group>.EQ(e => e.Id, groupId);
            var group = _database.GetCollection<Group>(_collectionName).FindOne(query);
            return group.Creator;
        }

        /*
        IEnumerable<Group> GetGroupsByUserId(ObjectId userId)
        {
            //var query = Query<Group>.In(  (e => e.AllMembers.ContainsKey(), id);
            //var group = _database.GetCollection<Group>(_collectionName).FindOne(query);
            return NotImplementedException();
        }
        */


        // For Admin Use Only!!!
        public void RemoveAllGroups()
        {
            _database.GetCollection<Group>(_collectionName).Drop();
        }

        public IEnumerable<Group> SearchGroup(string groupName)
        {
            List<Group> groups = new List<Group>();
            IEnumerable<Group> groupss = _database.GetCollection<Group>(_collectionName).FindAll();
            foreach (var item in groupss)
            {
                if (item.Name.ToLower().Contains(groupName.ToLower()))
                {
                    groups.Add(item);
                }
            }
            return groups;
        }

        public bool IsGroupOwner(string groupId, string userId)
        {
            var group = GetGroupById(new ObjectId(groupId));
            return group.Creator.ToString().Equals(userId);
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