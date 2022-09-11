using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TBlog.Core.Entities;
using TBlog.Core.Models;
using TBlog.Core.Models.ViewModels;
using TBlog.Core.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBlog.Core.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBlogRepository _blogService;
        private readonly IOptionsSnapshot<BlogSettings> _blogSettings;
        private readonly IOptionsSnapshot<EmailSettings> _emailSettings;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IBlogRepository blogService, IOptionsSnapshot<BlogSettings> blogSettings, IOptionsSnapshot<EmailSettings> emailSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _blogService = blogService;
            _blogSettings = blogSettings;
            _emailSettings = emailSettings;
        }

        [HttpGet]
        public IActionResult Index(string tab = "users")
        {
            var adminViewModel = new AdminViewModel();

            switch (tab)
            {
                case "users":
                    adminViewModel.UsersTabActive = true;
                    var users = _userManager.Users.OrderBy(o => o.Email);
                    foreach (var user in users)
                    {
                        var lockedOut = false;
                        if (user.LockoutEnd != null)
                        {
                            //lockedOut = (user.LockoutEnd > DateTime.UtcNow) ? true : false;
                            lockedOut = (user.LockoutEnd > DateTimeOffset.UtcNow) ? true : false;
                        }
                        adminViewModel.Users.Add(new UsersViewModel()
                        {
                            UserId = user.Id,
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            EmailConfirmed = user.EmailConfirmed,
                            LockedOut = lockedOut
                        });
                    }
                    break;

                case "groups":
                    adminViewModel.GroupsTabActive = true;
                    adminViewModel.UserCount = _userManager.Users.Count();
                    var groups = _roleManager.Roles.OrderBy(o => o.Name);
                    foreach (var group in groups)
                    {
                        adminViewModel.Groups.Add(new GroupsViewModel()
                        {
                            GroupId = group.Id,
                            GroupName = group.Name,
                            MembershipCount = _userManager.GetUsersInRoleAsync(group.Name).Result.Count
                            // Could get Roles Navigation Properties to work
                            //MembershipCount = _userManager.Users.Select(s => s.UserRoles.Select(g => g.RoleId == group.Id)).Count()
                        });
                    }
                    break;

                case "settings":
                    adminViewModel.SettingsTabActive = true;
                    var settings = new SettingsViewModel()
                    {
                        PageCount = _blogSettings.Value.PostsPerPage,
                        MailServer = _emailSettings.Value.MailServer,
                        MailPort = _emailSettings.Value.MailPort,
                        Sender = _emailSettings.Value.Sender,
                        SenderName = _emailSettings.Value.SenderName
                    };
                    adminViewModel.Settings = settings;
                    break;
            }

            return View(adminViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            var groups = await _userManager.GetRolesAsync(user);
            groups.Add("Add Comments"); // All enrolled users can add comments

            var editUserViewModel = new EditUserViewModel()
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailConfirmed = user.EmailConfirmed,
                Groups = groups
            };

            return View(editUserViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel editUserViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editUserViewModel);
            }

            var user = await _userManager.FindByIdAsync(editUserViewModel.UserId);

            if (user == null)
            {
                return View("NotFound");
            }

            user.Email = editUserViewModel.Email;
            user.FirstName = editUserViewModel.FirstName;
            user.LastName = editUserViewModel.LastName;
            user.EmailConfirmed = editUserViewModel.EmailConfirmed;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Admin", new { tab = "users" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(editUserViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UsersViewModel()
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailConfirmed = user.EmailConfirmed
            };

            return PartialView(userViewModel);
        }


        [HttpPost, ActionName("DeleteUser")]
        public async Task<IActionResult> DeleteUserConfirmed(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            await _blogService.DeleteUserDataAsync(Id);

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Error deleting user with ID '{Id}':");
            }

            return RedirectToAction("Index", "Admin", new { tab = "users" });
        }


        [HttpGet]
        public async Task<IActionResult> GroupMembers(string GroupName)
        {
            if (!await _roleManager.RoleExistsAsync(GroupName))
            {
                return NotFound();
            }

            var editGroupViewModel =  new EditGroupViewModel();
            var members = (await _userManager.GetUsersInRoleAsync(GroupName)).OrderBy(e => e.Email);
            foreach (var user in members)
            {
                editGroupViewModel.Members.Add(new UsersViewModel()
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                });
            }
            var nonMembers = _userManager.Users.Except(members).OrderBy(e => e.Email);
            foreach (var user in nonMembers)
            {
                editGroupViewModel.NonMembers.Add(new UsersViewModel()
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                });
            }

            editGroupViewModel.GroupName = GroupName;
            return View(editGroupViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditGroupUser(string Id, string Group)
        {
            var user = await _userManager.FindByIdAsync(Id);

            var result = (await _userManager.IsInRoleAsync(user, Group)) ? await _userManager.RemoveFromRoleAsync(user, Group) : await _userManager.AddToRoleAsync(user, Group);

            if (!result.Succeeded)
            {
                ViewBag.ExceptionMessage = "Unable to modify group membership";
                return View("Error"); 
            }

            return RedirectToAction("GroupMembers", "Admin", new { GroupName = Group });
        }
    }
}
