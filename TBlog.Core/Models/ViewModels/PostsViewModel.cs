using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TBlog.Core.Models.ViewModels
{
    public class PostsViewModel
    {
        public string Title { get; set; }

        public string Slug { get; set; }

        public string BriefDescription { get; set; }

        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTimeOffset PublishedDate { get; set; }

        public bool IsPublished { get; set; }

        public int CommentCount { get; set; }
    }
}
