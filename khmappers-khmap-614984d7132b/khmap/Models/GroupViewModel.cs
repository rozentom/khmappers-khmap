using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace khmap.Models
{
    public class GroupCreateViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "The {0} must be {2}-{1} characters long.")]
        [Display(Name = "Group Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "The {0} must be {2}-{1} characters long.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

    }

    public class GroupListViewModel
    {
        [Required]
        [Display(Name = "Group ID")]
        public ObjectId Id { get; set; }

        [Required]
        [Display(Name = "Group Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Creator")]
        public string Creator{ get; set; }

        [Required]
        [Display(Name = "Creation Time")]
        public DateTime CreationTime { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Group Size")]
        public int Size { get; set; }
    }

    public class GroupDetailsViewModel
    {
        [Required]
        [Display(Name = "Group ID")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Group Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Creator")]
        public string CreatorEmail { get; set; }

        [Required]
        [Display(Name = "Creation Time")]
        public DateTime CreationTime { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Members")]
        public Dictionary<ObjectId, GroupPermissionType> Members { get; set; }
    }

    public class GroupEditViewModel
    {
        [Required]
        [Display(Name = "Group ID")]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "The {0} must be {2}-{1} characters long.")]
        [Display(Name = "Group Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "The {0} must be {2}-{1} characters long.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }

    public class GroupViewModel
    {
        [Required]
        [Display(Name = "Group ID")]
        public ObjectId Id { get; set; }

        [Required]
        [Display(Name = "Group Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Creator")]
        public string CreatorEmail { get; set; }

        [Required]
        [Display(Name = "Creator ID")]
        public string CreatorId { get; set; }

        [Required]
        [Display(Name = "Creation Time")]
        public DateTime CreationTime { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Members")]
        public List<Tuple<ObjectId, string, GroupPermissionType>> Members { get; set; }
    }

    public class GroupAddUserViewModel
    {
        [Required]
        [Display(Name = "Group ID")]
        public ObjectId GroupId { get; set; }

        [Required]
        [Display(Name = "Group Name")]
        public ObjectId UserId { get; set; }

        [Required]
        [Display(Name = "Creator")]
        public string CreatorEmail { get; set; }

        [Required]
        [Display(Name = "Creation Time")]
        public DateTime CreationTime { get; set; }
    }

    public class GroupUserPermissionViewModel
    {
        [Required]
        [Display(Name = "Group ID")]
        public string GroupId { get; set; }

        [Required]
        [Display(Name = "User ID")]
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GroupPermissionType Permission { get; set; }
        public string PermissionString { get; set; }
    }

    public class GroupNameViewModel
    {
        [Required]
        [Display(Name = "Group ID")]
        public string GroupId { get; set; }

        [Required]
        [Display(Name = "Group Name")]
        public string Name { get; set; }
    }
}