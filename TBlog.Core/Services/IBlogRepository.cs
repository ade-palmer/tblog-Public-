using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBlog.Core.Entities;

namespace TBlog.Core.Services
{
    // This should probably be called IBlogRepository
    public interface IBlogRepository
    {
        Task<IEnumerable<Post>> GetPostsAsync(int currentPage, int pageSize, string searchString, string userId);
        Task<Post> GetPostBySlugAsync(string slug);
        Task<Post> GetPostByIdAsync(int id);
        Task<int> GetPostCountAsync(string searchString, string userId);  
        String GenerateSlug(string slug);
        Task<int> GetCommentCountAsync(int postId); 
        void AddPost(Post post); // Does not need to be async as just working with local context [Not I/O bound until Save called]
        void AddComment(Comment comment);
        void DeletePost(Post post);
        Task DeleteUserDataAsync(string Id);
        Task<IEnumerable<Comment>> GetUserCommentsByUserIdAsync(string Id);
        Task<IEnumerable<Post>> GetUserPostsByUserIdAsync(string Id);
        Task<bool> SaveChangesAsync(); // Async as I/O bound
    }
}
