namespace JobPortal.Models;

public class Seeker
{
    public Guid guid { get; set; } = Guid.NewGuid();

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string About { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty; 

    public DateTime DateOfBirth { get; set; } 

    public bool IsDeleted { get; set; } = false;

    public string ConnectionId { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public ICollection<SeekerSkills>? seekerSkills { get; set; }

    public ICollection<JobApplication>? jobApplications { get; set; }

    public User? user { get; set; }

    public ICollection<FileModel>? resumes { get; set; }

    public ICollection<Education>? educations { get; set; }

    public ICollection<Experience>? experiences { get; set; }
}
