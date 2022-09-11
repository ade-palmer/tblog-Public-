using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBlog.Core.Contexts;
using TBlog.Core.Entities;

namespace TBlog.Core.Services
{
    public class BlogRepository : IBlogRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // If null then soemthing went wrong and should be caught
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(int currentPage, int pageSize, string searchString, string userId)
        {
            if (userId == null)
            {
                var posts = await _context.Posts
                .OrderByDescending(d => d.PublishedDate)
                .Where(s => s.Title.Contains(searchString) || s.BriefDescription.Contains(searchString))
                .Where(s => s.IsPublished)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

                return posts;
            }
            else // Show unpublished post for logged in user as well
            {
                var posts = await _context.Posts
                .OrderByDescending(d => d.PublishedDate)
                .Where(s => s.Title.Contains(searchString) || s.BriefDescription.Contains(searchString))
                .Where(s => s.IsPublished || s.UserId == userId)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

                return posts;
            }
        }


        public async Task<Post> GetPostBySlugAsync(string slug)
        {
            var post = await _context.Posts
                .Where(p => p.Slug == slug)
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync();

            return post;
        }


        public async Task<Post> GetPostByIdAsync(int id)
        {
            var post = await _context.Posts
                .Where(p => p.PostId == id)
                .FirstOrDefaultAsync();

            return post;
        }


        public async Task<int> GetPostCountAsync(string searchString, string userId)
        {
            if (userId == null)
            {
                var postCount = await _context.Posts
                .Where(s => s.Title.Contains(searchString) || s.BriefDescription.Contains(searchString))
                .Where(s => s.IsPublished)
                .CountAsync();

                return postCount;
            }
            else // Count unpublished post for logged in user as well
            {
                var postCount = await _context.Posts
                .Where(s => s.Title.Contains(searchString) || s.BriefDescription.Contains(searchString))
                .Where(s => s.IsPublished || s.UserId == userId)
                .CountAsync();

                return postCount;
            }
        }


        public string GenerateSlug(string title)
        {
            var slug = title.Replace(" ", "-");
            slug = RemoveReservedUrlCharacters(slug);
            slug = CheckSlugInUse(slug);

            return slug;
        }


        private string RemoveReservedUrlCharacters(string slug)
        {
            var reservedCharacters = new List<string> { "!", "#", "$", "&", "'", "(", ")", "*", ",", "/", ":", ";", "=", "?", "@", "[", "]", "\"", "%", ".", "<", ">", "\\", "^", "_", "'", "{", "}", "|", "~", "`", "+" };

            foreach (var chr in reservedCharacters)
            {
                slug = slug.Replace(chr, "");
            }
            return slug;
        }


        private string CheckSlugInUse(string slug)
        {
            int blogSuffix = 1;
            string slugResult = slug;
            while (_context.Posts.Any(s => s.Slug == slugResult))
            {
                slugResult = slug + "-" + blogSuffix.ToString();
                blogSuffix++;
            }
            return slugResult;
        }


        public async Task<int> GetCommentCountAsync(int postId)
        {
            var commentCount = await _context.Comments
                .Where(i => i.PostId == postId)
                .CountAsync();

            return commentCount;
        }


        public void AddPost(Post post)
        {
            _context.Posts.Add(post);
        }


        public void AddComment(Comment comment)
        {
            _context.Comments.Add(comment);
        }


        public void DeletePost(Post post)
        {
            _context.Posts.Remove(post);
        }


        public async Task DeleteUserDataAsync(string Id)
        {
            //Remove Comments
            var userComments = await GetUserCommentsByUserIdAsync(Id);  // if we make this method sync you need .Result on the end which can cause deadlocks
            if (userComments != null)
            {
                _context.Comments.RemoveRange(userComments);
            }
            //Remove Posts
            var userPosts = await GetUserPostsByUserIdAsync(Id);
            if (userPosts != null)
            {
                _context.Posts.RemoveRange(userPosts);
            }
        }


        public async Task<IEnumerable<Comment>> GetUserCommentsByUserIdAsync(string Id)
        {
            var comments = await _context.Comments.Where(u => u.UserId == Id).ToListAsync();
            return comments;
        }


        public async Task<IEnumerable<Post>> GetUserPostsByUserIdAsync(string Id)
        {
            var posts = await _context.Posts.Where(u => u.UserId == Id).ToListAsync();
            return posts;
        }


        public async Task<bool> SaveChangesAsync()
        {
            // return true if 1 or more entities were changed. Async as I/O bound
            return (await _context.SaveChangesAsync() > 0);
        }
    }
}
