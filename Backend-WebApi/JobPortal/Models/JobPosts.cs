
using JobPortal.Models;

public class JobPost
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public string EmploymentPosition { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public string SalaryPackage { get; set; } = string.Empty;
    public Guid RecruiterID { get; set; }
    public DateTime PostedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastDate { get; set; }
    public ICollection<PostSkills>? requiredSkills { get; set; }
    public ICollection<Responsibilities>? responsibilities { get; set; }
    public ICollection<Requirements>? requirements { get; set; }
    public ICollection<JobApplication>? jobApplications { get; set; }
    public Recruiter? recruiter{ get; set; }
     public ICollection<FileModel>? resumes { get; set; }
}