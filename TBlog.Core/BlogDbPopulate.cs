using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBlog.Core.Contexts;
using TBlog.Core.Entities;

namespace TBlog.Core
{
    public static class BlogDbPopulate
    {
        public static void ClearSeededData(this ApplicationDbContext _dataManager)
        {
            var posts = _dataManager.Posts;
            _dataManager.RemoveRange(posts);
            _dataManager.SaveChanges();
        }


        public static async void ClearSeededUsers(this UserManager<ApplicationUser> _userManager)
        {
            if (_userManager.Users.Any())
            {
                var userList = _userManager.Users.ToList();
                foreach (var user in userList)
                {
                    await _userManager.DeleteAsync(user);
                }
            }
        }


        public static void CreateSeedRoles(this RoleManager<IdentityRole> _roleManager)
        {
            if (_roleManager.Roles.Any())
            {
                return;
            }

            var appRole = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Name = "Administrator"
                },
                new IdentityRole()
                {
                    Name = "Creator"
                }
            };

            foreach (var role in appRole)
            {
                IdentityResult result = _roleManager.CreateAsync(role).Result;
            }
        }


        public static void CreateSeedUsers(this UserManager<ApplicationUser> _userManager)
        {
            if (_userManager.Users.Any())
            {
                return;
            }

            var appUser = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                    UserName = "tony.green@hotmail.com",
                    Email = "tony.green@hotmail.com",
                    FirstName = "tony.green",
                    LastName = "Hotmail",
                    EnrollmentDateTime = DateTimeOffset.Parse("2019-01-01")
                },
                new ApplicationUser()
                {
                    UserName = "jane.childs@gmail.com",
                    Email = "jane.childs@gmail.com",
                    FirstName = "Adrian",
                    LastName = "Gmail",
                    EnrollmentDateTime = DateTimeOffset.Parse("2019-02-01")
                },
                new ApplicationUser()
                {
                    UserName = "bob.smith@hotmail.com",
                    Email = "bob.smith@hotmail.com",
                    FirstName = "Bob",
                    LastName = "Smith",
                    EnrollmentDateTime = DateTimeOffset.Parse("2019-03-01")
                },
                new ApplicationUser()
                {
                    UserName = "clare.butler@hotmail.com",
                    Email = "clare.butler@hotmail.com",
                    FirstName = "Clare",
                    LastName = "Butler",
                    EnrollmentDateTime = DateTimeOffset.Parse("2019-04-01")
                },
            };

            foreach (var user in appUser)
            {
                IdentityResult result = _userManager.CreateAsync(user, "SuperLongPassword#44").Result;
            }

            // Add one user as Administrator
            var defaultAdminUser = _userManager.Users.Where(u => u.UserName == "tony.green@hotmail.com").SingleOrDefault();
            IdentityResult adminResult = _userManager.AddToRoleAsync(defaultAdminUser, "Administrator").Result;

            // Add one user as Creator
            var defuaultCreatorUser = _userManager.Users.Where(u => u.UserName == "bob.smith@hotmail.com").SingleOrDefault();
            IdentityResult creatorResult = _userManager.AddToRoleAsync(defuaultCreatorUser, "Creator").Result;

        }


        public static void CreateSeedData(this ApplicationDbContext _dataManager)
        {
            if (_dataManager.Posts.Any())
            {
                return;
            }

            var ade_hotmail = _dataManager.Users.Where(u => u.UserName == "tony.green@hotmail.com").SingleOrDefault();
            var ade_gmail = _dataManager.Users.Where(u => u.UserName == "jane.childs@gmail.com").SingleOrDefault();
            var bob_smith = _dataManager.Users.Where(u => u.UserName == "bob.smith@hotmail.com").SingleOrDefault();
            var clare_butler = _dataManager.Users.Where(u => u.UserName == "clare.butler@hotmail.com").SingleOrDefault();

            var posts = new List<Post>()
            {
                new Post()
                {
                    Title = "Which programming language should I learn",
                    Slug = "which-programming-language-should-i-learn",
                    BriefDescription = "When I decided I wanted to become a developer the first decision I had to make was which programming language should i learn. This post details the decisions I made and why.",
                    Content = "<p>Which programming language should I learn covers my findings&nbsp;<br></p>",
                    PublishedDate = DateTimeOffset.Parse("2019-02-01"),
                    IsPublished = true,
                    UserId = ade_hotmail.Id,
                    Comments = new List<Comment>()
                    {
                        new Comment()
                        {
                            Content = "Realy liking this post. It covers all the stuff I wanted to know about",
                            PublishedDate = DateTimeOffset.Parse("2019-02-02 12:30"),
                            UserId = ade_gmail.Id
                        },
                        new Comment()
                        {
                            Content = "Not sure what you're on about",
                            PublishedDate = DateTimeOffset.Parse("2019-02-03 14:55"),
                            UserId = clare_butler.Id
                        }
                    }
                },
                new Post()
                {
                    Title = "Taghelpers",
                    Slug = "taghelpers",
                    BriefDescription = "How to create your own ASP.Net Core TagHelpers",
                    Content = "<p>How to create your own ASP.Net Core TagHelpers&nbsp;<br></p>",
                    PublishedDate = DateTimeOffset.Parse("2019-03-01"),
                    IsPublished = true,
                    UserId = bob_smith.Id,
                    Comments = new List<Comment>()
                    {
                        new Comment()
                        {
                            Content = "Need more background on TagHelpers",
                            PublishedDate = DateTimeOffset.Parse("2019-03-02 10:15"),
                            UserId = ade_hotmail.Id
                        },
                        new Comment()
                        {
                            Content = "That very concise",
                            PublishedDate = DateTimeOffset.Parse("2019-03-03 08:30"),
                            UserId = clare_butler.Id
                        }
                    }
                },
                new Post()
                {
                    Title = "Repository Pattern",
                    Slug = "repository-pattern",
                    BriefDescription = "Repository Pattern in a nutshell",
                    Content = "<p>Repository Pattern in a nutshell&nbsp;<br></p>",
                    PublishedDate = DateTimeOffset.Parse("2019-04-01"),
                    IsPublished = true,
                    UserId = clare_butler.Id,
                    Comments = new List<Comment>()
                    {
                        new Comment()
                        {
                            Content = "Have you got more info on the Onion framework",
                            PublishedDate = DateTimeOffset.Parse("2019-04-02"),
                            UserId = ade_hotmail.Id
                        },
                        new Comment()
                        {
                            Content = "I'm not sure this is best practice on a large project",
                            PublishedDate = DateTimeOffset.Parse("2019-04-03"),
                            UserId = bob_smith.Id
                        }
                    }
                },
                new Post()
                {
                    Title = "RedirectToAction",
                    Slug = "redirecttoaction",
                    BriefDescription = "Using an Ation as part of the redirect",
                    Content = "<p>See BlogController for example with Controller, Action and Route specified&nbsp;<br></p>",
                    PublishedDate = DateTimeOffset.Parse("2019-05-01"),
                    IsPublished = true,
                    UserId = bob_smith.Id,
                },
                new Post()
                {
                    Title = "ASP.Net Core Tempdata",
                    Slug = "aspnet-core-tempdata",
                    BriefDescription = "Using TempData instead of ViewData or ViewBag",
                    Content = "<p>Using TempData instead of ViewData or ViewBag&nbsp;<br></p>",
                    PublishedDate = DateTimeOffset.Parse("2019-06-01"),
                    IsPublished = true,
                    UserId = clare_butler.Id,
                },
                new Post()
                {
                    Title = "Modal Edit and Delete",
                    Slug = "modal-edit-and-delete",
                    BriefDescription = "Modal Edit and Delete",
                    Content = "<p>Modal Edit and Delete options and web links below&nbsp;<br></p>",
                    PublishedDate = DateTimeOffset.Parse("2019-07-01"),
                    IsPublished = true,
                    UserId = ade_gmail.Id,
                },
                new Post()
                {
                    Title = "How I studied",
                    Slug = "how-i-studied",
                    BriefDescription = "How I studied to get a job as a developer",
                    Content = "<p>How I studied to get a job as a developer&nbsp;<br></p>",
                    PublishedDate = DateTimeOffset.Parse("2019-04-05"),
                    IsPublished = true,
                    UserId = ade_hotmail.Id,
                },
                new Post()
                {
                    Title = "Jump to Anchor Link",
                    Slug = "jump-to-anchor-link",
                    BriefDescription = "Jump to Anchor Link with ASP.Net Core 3",
                    Content = "<p>Jump to Anchor Link with ASP.Net Core 3&nbsp;<br></p>",
                    PublishedDate = DateTimeOffset.Parse("2019-04-01"),
                    IsPublished = true,
                    UserId = ade_hotmail.Id,
                }
            };

            _dataManager.AddRange(posts);
            _dataManager.SaveChanges();
        }
    }
}
