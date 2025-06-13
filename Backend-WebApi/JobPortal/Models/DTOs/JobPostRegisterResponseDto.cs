public class JobPostRegisterResponseDto
{
    public Guid PostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid RecruiterId { get; set; }
    public DateTime PostedDate { get; set; }
    public DateTime LastDate { get; set; }
}
