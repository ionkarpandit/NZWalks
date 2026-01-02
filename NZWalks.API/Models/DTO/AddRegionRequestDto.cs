using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTO
{
    public class AddRegionRequestDto
    {
        [Required]  // Data annotation to ensure Code is provided
        [MinLength(3, ErrorMessage = "Code has to be a minimum 3 characters")]
        [MaxLength(3, ErrorMessage = "Code has to be a maximum 3 characters")]
        public string Code { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Name has to be a maximum 100 characters")]
        public string Name { get; set; }
        public string? RegionImageUrl { get; set; }
    }
}
