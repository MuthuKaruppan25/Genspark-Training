
using JobPortal.Models;

public class Education
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public string courseName { get; set; } = string.Empty;
    public string collegeName { get; set; } = string.Empty;
    public double grade { get; set; }
    public Guid SeekerId { get; set; }
     public Seeker? seeker { get; set; }
}