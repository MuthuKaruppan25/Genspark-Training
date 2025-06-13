using System.ComponentModel.DataAnnotations;

public class FileGetRequestDto
{
    [Required]
    public Guid SeekerId { get; set; }
    [Required]
    public Guid JobPostId { get; set; }
}