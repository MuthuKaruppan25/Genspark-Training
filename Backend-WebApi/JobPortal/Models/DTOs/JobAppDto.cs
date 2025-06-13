public class JobApplicationDto
{
    [TextValidator]
    public string JobTitle { get; set; } = string.Empty;
    [TextValidator]
    public string JobDescription { get; set; } = string.Empty;
    public DateTime AppliedOn { get; set; }
    public string Status { get; set; } = string.Empty;
}