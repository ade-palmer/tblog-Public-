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


namespace TBlog.Core.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private readonly IBlogRepository _blogService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptionsSnapshot<BlogSettings> _settings;

        public BlogController(IBlogRepository blogService, UserManager<ApplicationUser> userManager, IOptionsSnapshot<BlogSettings> settings)
        {
            _blogService = blogService;
            _userManager = userManager;
            _settings = settings;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString = "", int currentPage = 1)
        {
            var paginationViewModel = new PaginationViewModel()
            {
                CurrentPage = currentPage,
                Count = await _blogService.GetPostCountAsync(searchString, _userManager.GetUserId(User)),
                PageSize = _settings.Value.PostsPerPage,
                SearchString = searchString
            };

            var posts = await _blogService.GetPostsAsync(paginationViewModel.CurrentPage, paginationViewModel.PageSize, searchString, _userManager.GetUserId(User));

            var postsViewModel = new List<PostsViewModel>();

            foreach (var post in posts)
            {
                postsViewModel.Add(new PostsViewModel()
                {
                    Title = post.Title,
                    Slug = post.Slug,
                    BriefDescription = post.BriefDescription,
                    PublishedDate = post.PublishedDate.ToLocalTime(),
                    IsPublished = post.IsPublished,
                    CommentCount = await _blogService.GetCommentCountAsync(post.PostId)
                });
            }

            // Add Post Results to PaginationViewModel       
            paginationViewModel.Posts = postsViewModel;

            return View(paginationViewModel);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Post(string slug)
        {
            var post = await _blogService.GetPostBySlugAsync(slug);

            //TODO: This does cause an issue if the post owner adds a comment when a post is not published
            if (post == null || (!post.IsPublished && post.UserId != _userManager.GetUserId(User)))
            {
                return NotFound();
            }

            var postViewModel = new PostViewModel()
            {
                PostId = post.PostId,
                Title = post.Title,
                Slug = post.Slug,
                Content = post.Content,
                PublishedDate = post.PublishedDate.ToLocalTime(),
                PublishedBy = post.User.FirstName + " " + post.User.LastName,
                IsEditor = _userManager.GetUserId(User) == post.UserId,
                IsPublished = post.IsPublished,
                CommentCount = await _blogService.GetCommentCountAsync(post.PostId),
                Comments = new List<CommentViewModel>()
            };

            foreach (var comment in post.Comments)
            {
                postViewModel.Comments.Add(new CommentViewModel()
                {
                    CommentId = comment.CommentId,
                    Content = comment.Content,
                    PublishedDate = comment.PublishedDate.ToLocalTime(),
                    PublishedBy = comment.User.FirstName + " " + comment.User.LastName,
                    IsEditor = _userManager.GetUserId(User) == comment.UserId
                });
            }

            return View(postViewModel);
        }


        [HttpGet]
        [Authorize(Roles = "Administrator, Creator")]
        public IActionResult AddPost()
        {
            return View(new AddPostViewModel());
        }


        [HttpPost]
        [Authorize(Roles = "Administrator, Creator")]
        public async Task<IActionResult> AddPost(AddPostViewModel addPostViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var dateTimeNow = DateTimeOffset.Now;
            var post = new Post
            {
                Title = addPostViewModel.Title,
                Slug = _blogService.GenerateSlug(addPostViewModel.Title),
                BriefDescription = addPostViewModel.BriefDescription,
                Content = addPostViewModel.Content,
                PublishedDate = dateTimeNow,
                ModifiedDate = dateTimeNow,
                IsPublished = addPostViewModel.IsPublished,
                UserId = _userManager.GetUserId(User)
            };

            _blogService.AddPost(post);
            
            await _blogService.SaveChangesAsync();

            return RedirectToAction("Index", "Blog");
        }


        [HttpGet]
        public async Task<IActionResult> EditPost(int Id)
        {
            var post = await _blogService.GetPostByIdAsync(Id);

            if (post == null)
            {
                return NotFound();
            }

            if (_userManager.GetUserId(User) != post.UserId)
            {
                return RedirectToAction("Index", "Blog");
            }

            var editPostViewModel = new EditPostViewModel()
            {
                PostId = post.PostId,
                Title = post.Title,
                Slug = post.Slug,
                BriefDescription = post.BriefDescription,
                Content = post.Content,
                IsPublished = post.IsPublished
            };

            return View(editPostViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditPost(EditPostViewModel editPostViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editPostViewModel);
            }

            var post = await _blogService.GetPostByIdAsync(editPostViewModel.PostId);

            if (post.Title != editPostViewModel.Title)
            {
                post.Title = editPostViewModel.Title;
                post.Slug = _blogService.GenerateSlug(editPostViewModel.Title);
            }
            post.BriefDescription = editPostViewModel.BriefDescription;
            post.Content = editPostViewModel.Content;
            post.ModifiedDate = DateTimeOffset.Now;
            post.IsPublished = editPostViewModel.IsPublished;

            await _blogService.SaveChangesAsync();

            return RedirectToAction("Post", "Blog", new { slug = post.Slug });
        }


        [HttpGet]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var post = await _blogService.GetPostByIdAsync(postId);

            if (post == null)
            {
                return NotFound();
            }

            if (_userManager.GetUserId(User) != post.UserId)
            {
                return RedirectToAction("Index", "Blog");
            }

            var deletePostViewModel = new DeletePostViewModel()
            {
                PostId = post.PostId,
                Title = post.Title,
                BriefDescription = post.BriefDescription
            };

            return PartialView(deletePostViewModel);
        }


        [HttpPost, ActionName("DeletePost")]
        public async Task<IActionResult> DeletePostConfirmed(int postId)
        {
            var post = await _blogService.GetPostByIdAsync(postId);

            if (post == null)
            {
                return NotFound();
            }

            _blogService.DeletePost(post);

            await _blogService.SaveChangesAsync();

            return RedirectToAction("Index", "Blog");
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(int PostId, string Content, string returnUrl)
        {
            if (Content != null && Content != "")
            {
                var comment = new Comment()
                {
                    Content = Content,
                    PublishedDate = DateTimeOffset.Now,
                    UserId = _userManager.GetUserId(User),
                    PostId = PostId
                };

                _blogService.AddComment(comment);

                await _blogService.SaveChangesAsync();
            }
            return LocalRedirect(returnUrl ?? "/");
        }
    }
}
