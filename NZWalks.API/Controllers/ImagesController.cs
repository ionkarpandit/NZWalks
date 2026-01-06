using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        // POST: api/images
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadRequestDto imageUploadRequestDto)
        {
            ValidateFileUpload(imageUploadRequestDto);

            if (ModelState.IsValid)
            {
                // Convert DTO to domain model
                var imageDomainModel = new Image
                {
                    File = imageUploadRequestDto.File,
                    FileName = imageUploadRequestDto.FileName,
                    FileDescription = imageUploadRequestDto.FileDescription,
                    FileExtension = Path.GetExtension(imageUploadRequestDto.File.FileName),
                    FileSizeInBytes = imageUploadRequestDto.File.Length,
                    // In a real application, you would save the file to a storage location and set the FilePath accordingly
                    //FilePath = Path.Combine("Images", imageUploadRequestDto.FileName + Path.GetExtension(imageUploadRequestDto.File.FileName))
                };

                await imageRepository.Upload(imageDomainModel);

                return Ok(imageDomainModel);
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
