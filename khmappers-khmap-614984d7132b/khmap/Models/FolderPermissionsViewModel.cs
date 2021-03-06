﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace khmap.Models
{
    public class FolderPermissionsViewModel
    {
        public MapFolder Folder { get; set; }
        public KeyValuePair<ApplicationUser, MapPermissionType> Owner { get; set; }
        public Dictionary<ApplicationUser, MapPermissionType> Users { get; set; }
        public Dictionary<Group, MapPermissionType> Groups { get; set; }
        public MapPermissionType AllUsers { get; set; }
    }

    public class AddUserToFolderViewModel
    {
        [Required]
        [Display(Name = "Map ID")]
        public string FId { get; set; }

        [Display(Name = "User ID")]
        public string UId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "User Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Permission")]
        public MapPermissionType Permission { get; set; }
    }

    public class FolderUserPermissionViewModel
    {
        [Required]
        [Display(Name = "Folder ID")]
        public string FId { get; set; }

        [Display(Name = "User ID")]
        public string UId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "User Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Permission")]
        public MapPermissionType Permission { get; set; }
        public string PermissionString { get; set; }
    }


    public class FolderGroupPermissionViewModel
    {
        [Required]
        [Display(Name = "Folder ID")]
        public string FId { get; set; }

        [Display(Name = "Group ID")]
        public string GId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Owner Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Group Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Permission")]
        public MapPermissionType Permission { get; set; }

        public string PermissionString { get; set; }
    }

}