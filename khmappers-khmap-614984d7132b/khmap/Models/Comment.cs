using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace khmap.Models
{
    public class Comment
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public string MapId { get; set; }
        public List<CommentModel> Comments { get; set; }
    }


    public class CommentModel
    {
        public string Id { get; set; }
        public string Parent { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string Content { get; set; }
        public string Fullname { get; set; }
        public string ProfilePic { get; set; }
        public bool CreatedByAdmin { get; set; }
        public bool CreatedByCurrentUser { get; set; }
        public int UpvoteCount { get; set; }
        public bool UserHasUpvoted { get; set; }
        public string CreatorId { get; set; }
        public HashSet<string> Voters { get; set; }
    }

    public class JsonCommentModel
    {
        public string Id { get; set; }
        public string Parent { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
        public string Content { get; set; }
        public string Fullname { get; set; }
        public string ProfilePic { get; set; }
        public bool CreatedByAdmin { get; set; }
        public bool CreatedByCurrentUser { get; set; }
        public int UpvoteCount { get; set; }
        public bool UserHasUpvoted { get; set; }
    }
}