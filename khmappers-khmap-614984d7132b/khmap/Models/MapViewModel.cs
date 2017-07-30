using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace khmap.Models
{
    public class MapCreateViewModel
    {
        [Required]
        [Display(Name = "Map Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }

    }

    public class MapEditViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "The {0} must be {2}-{1} characters long.")]
        [Display(Name = "Map Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "The {0} must be {2}-{1} characters long.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Path")]
        public string Path
        {
            get; set;
        }
    }

    public class MapDeleteViewModel
    {
        [Required]
        [Display(Name = "Map ID")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Map Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Creator Email")]
        public string CreatorEmail { get; set; }

        [Required]
        [Display(Name = "Creation Time")]
        public DateTime CreationTime { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }

    public class MapViewModel
    {
        [Required]
        [Display(Name = "Map ID")]
        public ObjectId Id { get; set; }

        [Required]
        [Display(Name = "Map Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Creator Id")]
        public ObjectId CreatorId { get; set; }

        [Required]
        [Display(Name = "Creator Email")]
        public string CreatorEmail { get; set; }

        [Required]
        [Display(Name = "Creation Time")]
        public DateTime CreationTime { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Model")]
        public BsonDocument Model { get; set; }

        [Required]
        [Display(Name = "Archive")]
        public Queue<MapVersion> ModelArchive { get; set; }
    }

    public class MapMiniViewModel
    {
        [Required]
        [Display(Name = "Map ID")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Map Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Creator ID")]
        public string CreatorId { get; set; }

        [Required]
        [Display(Name = "Creation Time")]
        public DateTime CreationTime { get; set; }
    }

    public class FolderMiniViewModel
    {
        [Required]
        [Display(Name = "Folder ID")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Folder Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Creator ID")]
        public string CreatorId { get; set; }

        [Required]
        [Display(Name = "Creation Time")]
        public DateTime CreationTime { get; set; }
    }

    public class SaveMapViewModel
    {
        [Required]
        public string Model { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "The {0} must be {2}-{1} characters long.")]
        [Display(Name = "Map Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "The {0} must be {2}-{1} characters long.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}