using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TBlog.Core.Models.ViewModels
{
    public class EditPostViewModel
    {
        public int PostId { get; set; }

        [Required]
        [StringLength(256)]
        public string Title { get; set; }

        public string Slug { get; set; }

        [Required]
        [Display(Name = "Brief Description")]
        public string BriefDescription { get; set; }

        [Required]
        [Display(Name = "Blog Content")]
        public string Content { get; set; }

        public DateTimeOffset ModifiedDate { get; set; }

        [Display(Name = "Publish this Post?")]
        public bool IsPublished { get; set; }

    }
}
