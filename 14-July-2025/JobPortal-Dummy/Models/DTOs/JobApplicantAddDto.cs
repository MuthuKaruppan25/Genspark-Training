
using System.ComponentModel.DataAnnotations;

public class JobApplicantAddDto
{
    [Required]
    public Guid JobPostId { get; set; }
    [EmailValidation]
    public string username { get; set; } = string.Empty;
    [FileValidation]
    public IFormFile Resume { get; set; } = null!; 
}

public class StatusUpdate
{
    public string status { get; set; } = string.Empty;
}