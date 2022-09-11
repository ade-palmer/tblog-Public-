using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBlog.Core.Entities;

namespace TBlog.Core.Contexts
{
    public static class ModelBuilderExtensions
    {
        public static void SeedDefaultData(this ModelBuilder modelBuilder)
        {
            const string ADMIN_ROLE_ID = "b18b99c0-ab65-5af8-cd17-03bd9344e575";
            const string CREATOR_ROLE_ID = "c28b99c0-4b65-daf8-3d17-05bd9344e575";
            const string ADMIN_USER_ID = "a18b99c0-aa65-4af8-bd17-02bd9344e575";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = ADMIN_ROLE_ID,
                    Name = "Administrator",
                    NormalizedName = "Administrator".ToUpper()
                },
                new IdentityRole
                {
                    Id = CREATOR_ROLE_ID,
                    Name = "Creator",
                    NormalizedName = "Creator".ToUpper()
                }
            );

            var hasher = new PasswordHasher<ApplicationUser>();
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = ADMIN_USER_ID,
                    UserName = "administrator@tblog.co.uk",
                    NormalizedUserName = "administrator@tblog.co.uk".ToUpper(),
                    Email = "administrator@tblog.co.uk",
                    NormalizedEmail = "administrator@tblog.co.uk".ToUpper(),
                    FirstName = "Administrator",
                    EnrollmentDateTime = DateTimeOffset.Now,
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "ComplexPasswordHere#66"),
                    SecurityStamp = string.Empty
                }
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = ADMIN_ROLE_ID,
                    UserId = ADMIN_USER_ID
                }
            );
        }

        public static void SeedTestData(this ModelBuilder modelBuilder)
        {
            const string USER1_ID = "418bA9c0-abD5-5bf8-cd47-03bd5344e575";
            const string USER2_ID = "c38b99c0-4b65-dbf8-3a17-05bd9544e575";
            const string USER3_ID = "a20b99c0-ad65-4ba8-bd33-02b44344e575";
            const string USER4_ID = "c18b99c0-a345-4b18-a317-02456344e575";
            const string CREATOR_ROLE_ID = "c28b99c0-4b65-dbf8-3d17-05bd9344e575";

            var hasher = new PasswordHasher<ApplicationUser>();
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = USER1_ID,
                    UserName = "bob.smith@hotmail.com",
                    NormalizedUserName = "bob.smith@hotmail.com".ToUpper(),
                    Email = "bob.smith@hotmail.com",
                    NormalizedEmail = "bob.smith@hotmail.com".ToUpper(),
                    FirstName = "Bob",
                    LastName = "Smith",
                    EnrollmentDateTime = DateTimeOffset.Now,
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "ComplexPasswordHere#66"),
                    SecurityStamp = string.Empty
                },
                new ApplicationUser
                {
                    Id = USER2_ID,
                    UserName = "tony.green@hotmail.com",
                    NormalizedUserName = "tony.green@hotmail.com".ToUpper(),
                    Email = "tony.green@hotmail.com",
                    NormalizedEmail = "tony.green@hotmail.com".ToUpper(),
                    FirstName = "Ade",
                    LastName = "Hotmail",
                    EnrollmentDateTime = DateTimeOffset.Now,
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "ComplexPasswordHere#66"),
                    SecurityStamp = string.Empty
                },
                new ApplicationUser
                {
                    Id = USER3_ID,
                    UserName = "jane.childs@gmail.com",
                    NormalizedUserName = "jane.childs@gmail.com".ToUpper(),
                    Email = "jane.childs@gmail.com",
                    NormalizedEmail = "jane.childs@gmail.com".ToUpper(),
                    FirstName = "Ade",
                    LastName = "Gmail",
                    EnrollmentDateTime = DateTimeOffset.Now,
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "ComplexPasswordHere#66"),
                    SecurityStamp = string.Empty
                },
                new ApplicationUser
                {
                    Id = USER4_ID,
                    UserName = "clare.harris@hotmail.com",
                    NormalizedUserName = "clare.harris@hotmail.com".ToUpper(),
                    Email = "clare.harris@hotmail.com",
                    NormalizedEmail = "clare.harris@hotmail.com".ToUpper(),
                    FirstName = "Clare",
                    LastName = "Harris",
                    EnrollmentDateTime = DateTimeOffset.Now,
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "ComplexPasswordHere#66"),
                    SecurityStamp = string.Empty
                }
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = CREATOR_ROLE_ID,
                    UserId = USER2_ID
                }
            );

            modelBuilder.Entity<Post>().HasData(
                new Post
                {
                    PostId = 1,
                    Title = "Which programming language should I learn",
                    Slug = "which-programming-language-should-i-learn",
                    BriefDescription = "When I decided I wanted to become a developer the first decision I had to make was which programming language should i learn. This post details the decisions I made and why.",
                    Content = "<p>Which programming language should I learn covers my findings&nbsp;<br></p>",
                    PublishedDate = DateTimeOffset.Parse("2019-02-01"),
                    IsPublished = true,
                    UserId = USER1_ID
                },
                new Post
                {
                    PostId = 2,
                    Title = "Taghelpers",
                    Slug = "taghelpers",
                    BriefDescription = "How to create your own ASP.Net Core TagHelpers",
                    Content = "<p>How to create your own ASP.Net Core TagHelpers&nbsp;<br></p>",
                    PublishedDate = DateTimeOffset.Parse("2019-03-01"),
                    IsPublished = true,
                    UserId = USER2_ID
                }
            );

            modelBuilder.Entity<Comment>().HasData(
                new Comment
                {
                    CommentId = 1,
                    Content = "Realy liking this post. It covers all the stuff I wanted to know about",
                    PublishedDate = DateTimeOffset.Parse("2019-02-02 12:30"),
                    UserId = USER2_ID,
                    PostId = 1
                },
                new Comment
                {
                    CommentId = 2,
                    Content = "Realy liking this also so, so, so",
                    PublishedDate = DateTimeOffset.Parse("2019-02-02 12:30"),
                    UserId = USER3_ID,
                    PostId = 1
                }
            );
        }
    }
}
