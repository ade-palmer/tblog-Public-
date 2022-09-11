using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TBlog.Core.Models.ViewModels
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }

        public string Content { get; set; }

        [DisplayFormat(DataFormatString = "{0:f}")]
        public DateTimeOffset PublishedDate { get; set; }

        public string PublishedBy { get; set; }

        public bool IsEditor { get; set; }
    }
}
