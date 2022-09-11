using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TBlog.Core.Models.ViewModels
{
    public class PostViewModel
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTimeOffset PublishedDate { get; set; }
        public string PublishedBy { get; set; }
        public bool IsEditor { get; set; }
        public bool IsPublished { get; set; }

        public int CommentCount { get; set; }
        public ICollection<CommentViewModel> Comments { get; set; }
    }
}
