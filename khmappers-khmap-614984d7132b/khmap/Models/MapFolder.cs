using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using khmap.Models;

namespace khmap.Models
{   
    public class MapFolder
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public HashSet<ObjectId> idOfMapsInFolder { get; set; }
        public HashSet<ObjectId> idOfSubFolders { get; set; }
        public ObjectId Creator { get; set; }
        public DateTime CreationTime { get; set; }
        public string Description { get; set; }
        public BsonDocument Model { get; set; }
        public MapPermissions Permissions { get; set; }
        public HashSet<ObjectId> Followers { get; set; }
        [BsonIgnoreIfNull]
        public ObjectId ParentDierctory { get; set; }
        public ObjectId FirstFolderOfUser { get; set; } // if this field wont hold the default objectid it's the first folder of the user with this id  


    }
}