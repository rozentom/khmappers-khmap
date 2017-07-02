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
    public class UserDB
    {
        private readonly Settings _settings;
        private readonly MongoDatabase _database;
        private readonly string _collectionName;

        public UserDB(Settings settings)
        {
            _settings = settings;
            _collectionName = _settings.UsersCollectionName;
            _database = Connect();
        }

        public void AddUser(ApplicationUser user)
        {
            _database.GetCollection<ApplicationUser>(_collectionName).Save(user);
        }

        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            var users = _database.GetCollection<ApplicationUser>(_collectionName).FindAll();
            return users;
        }

        public ApplicationUser GetUserById(ObjectId id)
        {
            var query = Query<ApplicationUser>.EQ(e => e.Id, id.ToString());
            var user = _database.GetCollection<ApplicationUser>(_collectionName).FindOne(query);
            return user;
        }

        public ApplicationUser GetUserByEmail(string email)
        {
            var query = Query<ApplicationUser>.EQ(e => e.Email, email);
            var user = _database.GetCollection<ApplicationUser>(_collectionName).FindOne(query);
            return user;
        }

        public bool IsUserlExist(ObjectId userId)
        {
            var user = GetUserById(userId);
            return user != null;
        }

        public bool IsEmailExist(string email)
        {
            var query = Query<ApplicationUser>.EQ(e => e.Email, email);
            var user = _database.GetCollection<ApplicationUser>(_collectionName).FindOne(query);
            return user != null;
        }

        public bool RemoveUser(ObjectId id)
        {
            var query = Query<ApplicationUser>.EQ(e => e.Id, id.ToString());
            var result = _database.GetCollection<ApplicationUser>(_collectionName).Remove(query);

            return GetUserById(id) == null;
        }

        public void UpdateUser(ApplicationUser user)
        {
            var query = Query<ApplicationUser>.EQ(e => e.Id, user.Id);
            var update = Update<ApplicationUser>.Replace(user); // update modifiers
            _database.GetCollection<ApplicationUser>(_collectionName).Update(query, update);
        }


        // For Admin Use Only!!!
        public void RemoveAllUsers()
        {
            _database.GetCollection<ApplicationUser>(_collectionName).Drop();
        }

        public IEnumerable<ApplicationUser> SearchUser(string userName)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            IEnumerable<ApplicationUser> userss = _database.GetCollection<ApplicationUser>(_collectionName).FindAll();
            foreach (var item in userss)
            {
                if (item.FirstName.ToLower().Contains(userName.ToLower()))
                {
                    users.Add(item);
                }
            }
            return users;
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