using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace khmap.Models
{
    public class Reference
    {
        public Reference()
        {
            this.Title = "";
            this.Publication = "";
            this.Authors = new List<string>();
            this.Description = "";
            this.Link = "";
            this.CreationTime = DateTime.Now;
        }

        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public string Publication { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public ObjectId Creator { get; set; }
        public DateTime CreationTime { get; set; }
    }
}