using System.ComponentModel.DataAnnotations;

public class JobPostResponse
{
    [TextValidator]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [TextValidator]
    public string EmploymentType { get; set; } = string.Empty;
    [TextValidator]
    public string EmploymentPosition { get; set; } = string.Empty;
    public ICollection<LocationDto>? locations { get; set; }
    public int Minsalary { get; set; }
    public int Maxsalary { get; set; }
    [Required]
    public Guid RecruiterId { get; set; }
    [Required]
    public DateTime LastDate { get; set; }
    public ICollection<RequirementsAddDto>? requirements { get; set; }
    public ICollection<ResponsibilitiesAddDto>? responsibilities { get; set; }
    public ICollection<SkillRegisterDto>? skills { get; set; }
    
}