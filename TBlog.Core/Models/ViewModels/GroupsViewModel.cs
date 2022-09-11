using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBlog.Core.Models.ViewModels
{
    public class GroupsViewModel
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public int MembershipCount { get; set; }
    }
}
