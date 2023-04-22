using BlogLab.Models.Blog;

namespace BlogLab.Repository
{
    public interface IBlogRepository
    {
        public Task<BlogEntry> UpsertAsync(BlogCreate blogCreate, int applicationUserId);
        public Task<BlogEntry> GetAsync(int blogId);
        public Task<PagedResults<BlogEntry>> GetAllAsync(BlogPaging blogPaging);
        public Task<List<BlogEntry>> GetAllByUserIdAsync(int applicationUserId);
        public Task<List<BlogEntry>> GetAllFamousAsync();
        public Task<int> DeleteAsync(int blogId);
    }
}
