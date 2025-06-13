public class JobPostWithApplicantsDto
{
    public Guid JobPostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> RequiredSkills { get; set; } = new();
    public PagedResult<JobApplicantAddResponse> Applicants { get; set; } = new();
}