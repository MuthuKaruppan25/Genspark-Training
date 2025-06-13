public class JobApplicantAddResponse
{
    public DateTime AppliedOn { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = string.Empty;
    public Guid JobPostId { get; set; }
    public Guid SeekerId { get; set; }
    public string SeekerName { get; set; } = string.Empty;
    public List<string>? SeekerSkills { get; set; }
}