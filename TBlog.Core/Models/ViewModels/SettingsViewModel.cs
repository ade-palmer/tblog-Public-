using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TBlog.Core.Models.ViewModels
{
    public class SettingsViewModel
    {
        [Display(Name = "Posts per page")]
        public int PageCount { get; set; }

        [Display(Name = "Email Server")]
        public string MailServer { get; set; }

        [Display(Name = "Email Port")]
        public int MailPort { get; set; }

        [Display(Name = "Email Sender Name")]
        public string SenderName { get; set; }

        [Display(Name = "Email Sender")]
        public string Sender { get; set; }
    }
}
