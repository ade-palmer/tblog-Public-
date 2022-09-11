using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TBlog.Core.TagHelpers
{
    [HtmlTargetElement(Attributes = nameof(Confirmed))]
    public class ConfirmedTagHelper : TagHelper
    {
        public bool Confirmed { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Check if Not Confirmed
            if (!Confirmed)
            {
                output.SuppressOutput();
            }

        }
    }
}
