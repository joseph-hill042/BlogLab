using Microsoft.AspNetCore.Mvc;
using BlogLab.Repository;
using Microsoft.AspNetCore.Authorization;
using BlogLab.Models.Account;
using BlogLab.Models.Blog;
using System.IdentityModel.Tokens.Jwt;

namespace BlogLab.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IPhotoRepository _photoRepository;

        public BlogController(IBlogRepository blogRepository, IPhotoRepository photoRepository)
        {
            _blogRepository = blogRepository;
            _photoRepository = photoRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<BlogEntry>> Create(BlogCreate blogCreate)
        {
            int applicationUserId = int.Parse(
                User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value
            );

            if (blogCreate.PhotoId.HasValue)
            {
                var photo = await _photoRepository.GetAsync(blogCreate.PhotoId.Value);

                if (photo.ApplicationUserId != applicationUserId)
                {
                    return BadRequest("You did not upload the photo.");
                }
            }

            var blog = await _blogRepository.UpsertAsync(blogCreate, applicationUserId);
            return Ok(blog);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResults<BlogEntry>>> GetAll(
            [FromQuery] BlogPaging blogPaging
        )
        {
            var blogs = await _blogRepository.GetAllAsync(blogPaging);
            return Ok(blogs);
        }

        [HttpGet("{blogId}")]
        public async Task<ActionResult<BlogEntry>> Get(int blogId)
        {
            var blog = await _blogRepository.GetAsync(blogId);
            return Ok(blog);
        }

        [HttpGet("user/{applicationUserId}")]
        public async Task<ActionResult<List<BlogEntry>>> GetAllByUserId(int applicationUserId)
        {
            var blogs = await _blogRepository.GetAllByUserIdAsync(applicationUserId);
            return Ok(blogs);
        }

        [HttpGet("famous")]
        public async Task<ActionResult<List<BlogEntry>>> GetAllFamous()
        {
            var blogs = await _blogRepository.GetAllFamousAsync();
            return Ok(blogs);
        }

        [Authorize]
        [HttpDelete("{blogId}")]
        public async Task<ActionResult<int>> Delete(int blogId)
        {
            int applicationUserId = int.Parse(
                User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value
            );

            var blog = await _blogRepository.GetAsync(blogId);

            if (blog == null)
            {
                return BadRequest("Blog does not exist.");
            }

            if (blog.ApplicationUserId != applicationUserId)
            {
                return BadRequest("You did not create this blog.");
            }

            var affectedRows = await _blogRepository.DeleteAsync(blogId);
            return Ok(affectedRows);
        }
    }
}
