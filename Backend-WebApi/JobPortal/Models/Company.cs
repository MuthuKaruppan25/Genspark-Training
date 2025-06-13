
namespace JobPortal.Models;

public class Company
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public string CompanyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public Guid IndustryTypeId { get; set; } 
    public bool IsDeleted { get; set; } = false;
    public IndustryType? industryType { get; set; }
    public ICollection<Recruiter>? recruiters { get; set; }
    public ICollection<Address>? locations { get; set; }

}