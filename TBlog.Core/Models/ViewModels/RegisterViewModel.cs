using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TBlog.Core.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address", Prompt = "name@address.com")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(256)]
        [Display(Name = "First Name", Prompt = "Required")]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        [StringLength(256)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", Prompt = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password", Prompt = "Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public DateTimeOffset Enrollment { get; set; } = DateTime.Now;
    }
}
