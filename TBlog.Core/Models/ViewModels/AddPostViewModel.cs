using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TBlog.Core.Models.ViewModels
{
    public class AddPostViewModel
    {
        [Required]
        [StringLength(256)]
        [Display(Prompt = "The title as it appears when published")]
        public string Title { get; set; }

        public string Slug { get; set; }

        [Required]
        [Display(Name = "Brief Description", Prompt = "Required")]
        public string BriefDescription { get; set; }

        [Required]
        [Display(Name = "Blog Content")]
        public string Content { get; set; }

        public DateTimeOffset PublishedDate { get; set; }

        public DateTimeOffset ModifiedDate { get; set; }

        [Display(Name = "Publish this Post?")]
        public bool IsPublished { get; set; } = true;

        public string UserId { get; set; }

    }
}
