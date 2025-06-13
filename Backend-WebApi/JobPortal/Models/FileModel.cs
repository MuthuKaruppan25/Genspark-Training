using JobPortal.Models;

public class FileModel
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public string Type { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public Guid? JobPostId { get; set; }
    public JobPost? JobPost { get; set; }

    public Guid? SeekerId { get; set; }
    public Seeker? Seeker { get; set; }
}