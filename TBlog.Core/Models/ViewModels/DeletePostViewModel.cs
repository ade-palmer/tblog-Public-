using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBlog.Core.Models.ViewModels
{
    public class DeletePostViewModel
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string BriefDescription { get; set; }
    }
}
