public class SeekerWithApplicationsDto
{
    public Guid SeekerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public List<JobApplicationDto> Applications { get; set; } = new();
}