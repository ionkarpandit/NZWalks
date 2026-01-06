using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        // POST: api/images
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadRequestDto imageUploadRequestDto)
        {
            ValidateFileUpload(imageUploadRequestDto);

            if (ModelState.IsValid)
            {
                // Simulate file saving process
            }

            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(ImageUploadRequestDto requestDto)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(requestDto.File.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("File", "Unsupported file type. Allowed types are: .jpg, .jpeg, .png, .gif");
            }

            var maxFileSizeInBytes = 10 * 1024 * 1024; // 5 MB
            if (requestDto.File.Length > maxFileSizeInBytes)
            {
                ModelState.AddModelError("File", "File size exceeds the maximum limit of 10 MB.");
            }
        }

    }
}
