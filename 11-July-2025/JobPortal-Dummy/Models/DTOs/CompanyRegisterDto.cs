
using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models.DTOs;

public class CompanyRegisterDto
{
    [TextValidator]
    public string CompanyName { get; set; } = string.Empty;
    [RequirementsValidator]
    public string Description { get; set; } = string.Empty;
    [UrlValidator]
    public string WebsiteUrl { get; set; } = string.Empty;
    [Required]
    public IndustryTypeRegister? industryTypeRegister { get; set; }
    [Required]
    public ICollection<AddressRegisterDto>? locations { get; set; }
 
}