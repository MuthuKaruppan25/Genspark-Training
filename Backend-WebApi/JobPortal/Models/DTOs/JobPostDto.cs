

using System.ComponentModel.DataAnnotations;

public class JobPostDto
{
    [TextValidator]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    [TextValidator]
    public string EmploymentType { get; set; } = string.Empty;
    [TextValidator]
    public string EmploymentPosition { get; set; } = string.Empty;
    [TextValidator]
    public string Location { get; set; } = string.Empty;
    [Required]
    public string SalaryPackage { get; set; } = string.Empty;
    [Required]
    public Guid RecruiterId { get; set; }
    [Required]
    public DateTime LastDate { get; set; }
    public ICollection<RequirementsAddDto>? requirements { get; set; }
    public ICollection<ResponsibilitiesAddDto>? responsibilities { get; set; }
    public ICollection<SkillRegisterDto>? skills { get; set; }
}