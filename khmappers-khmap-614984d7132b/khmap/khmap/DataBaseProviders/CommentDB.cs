using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using khmap.Models;

namespace khmap.DataBaseProviders
{
    public class CommentDB
    {
        private readonly Settings _settings;
        private readonly MongoDatabase _database;
        private readonly string _collectionName;

        public CommentDB(Settings settings)
        {
            _settings = settings;
            _collectionName = _settings.CommentsCollectionName;
            _database = Connect();
        }

        public void AddComment(Comment comment)
        {
            _database.GetCollection<Comment>(_collectionName).Save(comment);
        }

        public IEnumerable<Comment> GetAllComments()
        {
            var comments = _database.GetCollection<Comment>(_collectionName).FindAll();
            return comments;
        }

        public Comment GetCommentById(string id)
        {
            var query = Query<Comment>.EQ(e => e.Id, id);
            var comment = _database.GetCollection<Comment>(_collectionName).FindOne(query);
            return comment;
        }

        public bool RemoveComment(string id)
        {
            var query = Query<Comment>.EQ(e => e.Id, id);
            var result = _database.GetCollection<Comment>(_collectionName).Remove(query);
            return GetCommentById(id) == null;
        }

        public void UpdateComment(Comment comment)
        {
            var query = Query<Comment>.EQ(e => e.Id, comment.Id);
            var update = Update<Comment>.Replace(comment); // update modifiers
            _database.GetCollection<Comment>(_collectionName).Update(query, update);
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