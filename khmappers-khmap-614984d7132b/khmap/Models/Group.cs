using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace khmap.Models
{

    public enum GroupPermissionType
    {
        [Display(Name = "Owner")]
        Owner,      // Can add/remove members and managers. can delete the group

        [Display(Name = "Manager")]
        Manager,    // Can add/remove members

        [Display(Name = "Member")]
        Member
    };

    public class Group
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public ObjectId Creator { get; set; }
        public DateTime CreationTime { get; set; }
        public string Description { get; set; }
        public Dictionary<ObjectId, GroupPermissionType> Members { get; set; }
    }
}