using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace khmap.Models
{
    public class DetailsViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Joined at")]
        public DateTime CreationTime { get; set; }

        [Required]
        [Display(Name = "Reputation")]
        public double Reputation { get; set; }
    }
}