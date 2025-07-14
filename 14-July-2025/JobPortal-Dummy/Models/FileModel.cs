using JobPortal.Models;

public class FileModel
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty; // New field
    public string Type { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public Guid? JobPostId { get; set; }
    public JobPost? JobPost { get; set; }
    public Guid? SeekerId { get; set; }
    public Seeker? Seeker { get; set; }
}