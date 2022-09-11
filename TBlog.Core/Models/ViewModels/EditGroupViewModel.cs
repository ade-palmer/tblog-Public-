using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBlog.Core.Models.ViewModels
{
    public class EditGroupViewModel
    {
        public string GroupName { get; set; }
        public ICollection<UsersViewModel> Members { get; set; } = new List<UsersViewModel>();
        public ICollection<UsersViewModel> NonMembers { get; set; } = new List<UsersViewModel>();
    }
}
