using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TBlog.Core.Models.ViewModels
{
    public class EditUserViewModel
    {
        [Display(Name ="User ID")]
        public string UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email address confirmed?")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Groups")]
        public ICollection<string> Groups { get; set; } = new List<string>();
    }
}
