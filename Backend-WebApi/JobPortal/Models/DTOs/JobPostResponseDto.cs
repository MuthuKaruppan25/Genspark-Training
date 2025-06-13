public class JobPostResponse
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public string EmploymentPosition { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public string SalaryPackage { get; set; } = string.Empty;
    public Guid RecruiterID { get; set; }
    public DateTime PostedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastDate { get; set; }
    public ICollection<RequirementsAddDto>? requirements { get; set; }
    public ICollection<ResponsibilitiesAddDto>? responsibilities { get; set; }
    public ICollection<SkillRegisterDto>? skills { get; set; }
    
}