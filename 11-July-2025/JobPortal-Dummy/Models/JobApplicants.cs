namespace JobPortal.Models;

public class JobApplication
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public DateTime AppliedOn { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public Guid JobPostId { get; set; }
    public Guid SeekerId { get; set; }
    public JobPost? jobPost { get; set; }
    public Seeker? seeker { get; set; }
}