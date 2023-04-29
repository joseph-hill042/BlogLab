using System.IdentityModel.Tokens.Jwt;
using BlogLab.Models.BlogComment;
using BlogLab.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogLab.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogCommentController : ControllerBase
    {
        private readonly IBlogCommentRepository _blogCommentRepository;

        public BlogCommentController(IBlogCommentRepository blogCommentRepository)
        {
            _blogCommentRepository = blogCommentRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BlogComment>> Create(BlogCommentCreate blogCommentCreate)
        {
            int applicationUserId = int.Parse(
                User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value
            );
            var blogComment = await _blogCommentRepository.UpsertAsync(
                blogCommentCreate,
                applicationUserId
            );
            return Ok(blogComment);
        }

        [HttpGet("{blogId}")]
        public async Task<ActionResult<BlogComment>> GetAll(int blogId)
        {
            var blogComments = await _blogCommentRepository.GetAllAsync(blogId);
            return Ok(blogComments);
        }

        [Authorize]
        [HttpDelete("{blogCommentId}")]
        public async Task<ActionResult<int>> Delete(int blogCommentId)
        {
            int applicationUserId = int.Parse(
                User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value
            );
            var comment = await _blogCommentRepository.GetAsync(blogCommentId);
            if (comment == null)
                return BadRequest("Comment does not exist");
            if (applicationUserId != comment.ApplicationUserId)
                return BadRequest("You did not create this comment");
            var deleted = await _blogCommentRepository.DeleteAsync(blogCommentId);
            return Ok(deleted);
        }
    }
}
