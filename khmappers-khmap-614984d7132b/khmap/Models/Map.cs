using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace khmap.Models
{


    public enum MapPermissionType
    {
        [Display(Name = "ReadWrite")]
        RW,      // Can Edit map

        [Display(Name = "ReadOnly")]
        RO,    // Can view map only

        [Display(Name = "NoAccess")]
        NA // No Access
    };

    public class Map
    {
        [BsonId]
        public ObjectId Id { get;
            set; }
        public string Name { get; set; }
        public ObjectId Creator { get; set; }
        public DateTime CreationTime { get; set; }
        public string Description { get; set; }
        public BsonDocument Model { get; set; }
        public MapPermissions Permissions { get; set; }
        public HashSet<ObjectId> Followers { get; set; }
        public Queue<MapVersion> MapsArchive { get; set; }
    }

    public class MapVersion
    {
        public static readonly int VERSIONS = 5;
        public DateTime CreationTime { get; set; }
        public BsonDocument Model { get; set; }
    }

    public class MapPermissions
    {
        public KeyValuePair<ObjectId, MapPermissionType> Owner { get; set; }
        public Dictionary<ObjectId, MapPermissionType> Users { get; set; }
        public Dictionary<ObjectId, MapPermissionType> Groups { get; set; }
        public MapPermissionType AllUsers { get; set; }
    }


    class MapComparer : IEqualityComparer<Map>
    {

        public bool Equals(Map x, Map y)
        {
            return x.Id.ToString().Equals(y.Id.ToString());
        }

        public int GetHashCode(Map obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    class IDComparer : IEqualityComparer<ObjectId>
    {

        public bool Equals(ObjectId x, ObjectId y)
        {
            return x.ToString().Equals(y.ToString());
        }

        public int GetHashCode(ObjectId obj)
        {
            return obj.GetHashCode();
        }
    }

}