using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBlog.Core.Models.ViewModels
{
    public class AdminViewModel
    {
        public bool UsersTabActive { get; set; } = false;
        public bool GroupsTabActive { get; set; } = false;
        public bool SettingsTabActive { get; set; } = false;
        public int UserCount { get; set; }
        public ICollection<UsersViewModel> Users { get; set; } = new List<UsersViewModel>();
        public ICollection<GroupsViewModel> Groups { get; set; } = new List<GroupsViewModel>();
        public SettingsViewModel Settings { get; set; } = new SettingsViewModel();
    }
}
