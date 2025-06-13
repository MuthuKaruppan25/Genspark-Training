
namespace JobPortal.Models;

public class Seeker
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Experience { get; set; }
    public string About { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public string ConnectionId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public ICollection<SeekerSkills>? seekerSkills { get; set; }
    public ICollection<JobApplication>? jobApplications { get; set; }
    public User? user { get; set; }
    public ICollection<FileModel>? resumes { get; set; }

}