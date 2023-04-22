using System.Data;
using System.Data.SqlClient;
using BlogLab.Models.Blog;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace BlogLab.Repository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly IConfiguration _config;

        public BlogRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<int> DeleteAsync(int blogId)
        {
            int affectedRows = 0;
            using (
                var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"))
            )
            {
                await connection.OpenAsync();

                affectedRows = await connection.ExecuteAsync(
                    "Blog_Delete",
                    new { BlogId = blogId },
                    commandType: CommandType.StoredProcedure
                );
            }

            return affectedRows;
        }

        public async Task<PagedResults<BlogEntry>> GetAllAsync(BlogPaging blogPaging)
        {
            var results = new PagedResults<BlogEntry>();

            using (
                var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"))
            )
            {
                await connection.OpenAsync();

                using (
                    var multi = await connection.QueryMultipleAsync(
                        "Blog_All",
                        new
                        {
                            Offset = (blogPaging.Page - 1) * blogPaging.PageSize,
                            PageSize = blogPaging.PageSize
                        },
                        commandType: CommandType.StoredProcedure
                    )
                )
                {
                    results.Items = multi.Read<BlogEntry>();
                    results.TotalCount = multi.ReadFirst<int>();
                }
            }

            return results;
        }

        public async Task<List<BlogEntry>> GetAllByUserIdAsync(int applicationUserId)
        {
            IEnumerable<BlogEntry> blogEntries;

            using (
                var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"))
            )
            {
                await connection.OpenAsync();

                blogEntries = await connection.QueryAsync<BlogEntry>(
                    "Blog_GetByUserId",
                    new { ApplicationUserId = applicationUserId },
                    commandType: CommandType.StoredProcedure
                );
            }

            return blogEntries.ToList();
        }

        public async Task<List<BlogEntry>> GetAllFamousAsync()
        {
            IEnumerable<BlogEntry> famousBlogEntries;

            using (
                var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"))
            )
            {
                await connection.OpenAsync();

                famousBlogEntries = await connection.QueryAsync<BlogEntry>(
                    "Blog_GetAllFamous",
                    commandType: CommandType.StoredProcedure
                );
            }

            return famousBlogEntries.ToList();
        }

        public async Task<BlogEntry> GetAsync(int blogId)
        {
            BlogEntry blogEntry;

            using (
                var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"))
            )
            {
                await connection.OpenAsync();

                blogEntry = await connection.QueryFirstOrDefaultAsync<BlogEntry>(
                    "Blog_Get",
                    new { BlogId = blogId },
                    commandType: CommandType.StoredProcedure
                );
            }

            return blogEntry;
        }

        public async Task<BlogEntry> UpsertAsync(BlogCreate blogCreate, int applicationUserId)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("BlogId", typeof(int));
            dataTable.Columns.Add("Title", typeof(string));
            dataTable.Columns.Add("Content", typeof(string));
            dataTable.Columns.Add("PhotoId", typeof(int));

            dataTable.Rows.Add(
                blogCreate.BlogId,
                blogCreate.Title,
                blogCreate.Content,
                blogCreate.PhotoId
            );

            int? newBlogId;

            using (
                var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"))
            )
            {
                await connection.OpenAsync();

                newBlogId = await connection.ExecuteScalarAsync<int?>(
                    "Blog_Upsert",
                    new
                    {
                        Blog = dataTable.AsTableValuedParameter("dbo.BlogType"),
                        ApplicationUserId = applicationUserId
                    },
                    commandType: CommandType.StoredProcedure
                );
            }

            newBlogId = newBlogId ?? blogCreate.BlogId;

            BlogEntry blogEntry = await GetAsync(newBlogId.Value);

            return blogEntry;
        }
    }
}
