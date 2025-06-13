
using JobPortal.Models;

public class CompanyRegisterResponseDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public IndustryType? industryType { get; set; }
    public ICollection<Address>? locations { get; set; }

}