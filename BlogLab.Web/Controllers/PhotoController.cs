using Microsoft.AspNetCore.Mvc;
using BlogLab.Services;
using BlogLab.Repository;
using Microsoft.AspNetCore.Authorization;
using BlogLab.Models.Photo;
using System.IdentityModel.Tokens.Jwt;

namespace BlogLab.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoRepository _photoRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly IPhotoService _photoService;

        public PhotoController(
            IPhotoRepository photoRepository,
            IBlogRepository blogRepository,
            IPhotoService photoService
        )
        {
            _photoRepository = photoRepository;
            _blogRepository = blogRepository;
            _photoService = photoService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Photo>> UploadPhoto(IFormFile file)
        {
            int applicationUserId = int.Parse(
                User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value
            );
            var uploadResult = await _photoService.AddPhotoAsync(file);
            if (uploadResult.Error != null)
            {
                return BadRequest(uploadResult.Error.Message);
            }
            var photoCreate = new PhotoCreate
            {
                PublicId = uploadResult.PublicId,
                ImageUrl = uploadResult.SecureUrl.AbsoluteUri,
                Description = file.FileName
            };

            var photo = await _photoRepository.InsertAsync(photoCreate, applicationUserId);

            return Ok(photo);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<Photo>>> GetByApplicationUserId()
        {
            int applicationUserId = int.Parse(
                User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value
            );
            var photos = await _photoRepository.GetAllByUserIdAsync(applicationUserId);
            return Ok(photos);
        }

        [HttpGet("{photoId}")]
        public async Task<ActionResult<Photo>> Get(int photoId)
        {
            var photo = await _photoRepository.GetAsync(photoId);
            return Ok(photo);
        }

        [Authorize]
        [HttpDelete("{photoId}")]
        public async Task<ActionResult<int>> Delete(int photoId)
        {
            int applicationUserId = int.Parse(
                User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value
            );
            var photo = await _photoRepository.GetAsync(photoId);
            if (photo == null)
                return BadRequest("Photo does not exist");
            if (photo.ApplicationUserId != applicationUserId)
            {
                return BadRequest("You did not upload this photo");
            }
            var blogs = await _blogRepository.GetAllByUserIdAsync(applicationUserId);
            if (blogs.Any(b => b.PhotoId == photoId))
            {
                return BadRequest("You cannot delete a photo that is being used by a blog");
            }
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error == null)
            {
                var affectedRows = await _photoRepository.DeleteAsync(photoId);
                return Ok(affectedRows);
            }
            return BadRequest(result.Error.Message);
        }
    }
}
