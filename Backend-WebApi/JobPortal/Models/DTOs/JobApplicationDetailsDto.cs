public class JobApplicationDetailsDto
{
    
    public Guid ApplicationId { get; set; }
    public DateTime AppliedOn { get; set; }
    public string Status { get; set; } = string.Empty;


    public Guid SeekerId { get; set; }
    public string SeekerName { get; set; } = string.Empty;
    public List<string> SeekerSkills { get; set; } = new();

    
    public Guid JobPostId { get; set; }
    public string JobPostTitle { get; set; } = string.Empty;
    public List<string> RequiredSkills { get; set; } = new();

    
    public string RecruiterName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
}
