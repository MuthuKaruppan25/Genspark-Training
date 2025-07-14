using System.ComponentModel.DataAnnotations;

public class FileUploadDto
{
    [Required]
    public Guid SeekerId { get; set; }
    [Required]
    public Guid JobPostId { get; set; }
    [FileValidation]
    public IFormFile Resume { get; set; } = null!; 
}