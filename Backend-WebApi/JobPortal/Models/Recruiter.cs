
namespace JobPortal.Models;

public class Recruiter
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public Guid UserId { get; set; }
    public Guid CompanyId { get; set; }

    public User? user { get; set; }
    public Company? company { get; set; }
    public ICollection<JobPost>? jobPosts { get; set; }

}