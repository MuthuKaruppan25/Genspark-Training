
using JobPortal.Models;
using JobPortal.Models.DTOs;

public class CompanyMapper
{
    public Company MapCompany(CompanyRegisterDto dto, IndustryType industry)
    {
        return new Company
        {
            guid = Guid.NewGuid(),
            CompanyName = dto.CompanyName,
            Description = dto.Description,
            WebsiteUrl = dto.WebsiteUrl,
            IndustryTypeId = industry.guid,
            IsDeleted = false,
            industryType = industry,
            locations = new List<Address>() 
        };
    }
}
