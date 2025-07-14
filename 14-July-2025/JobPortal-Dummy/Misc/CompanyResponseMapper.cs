using JobPortal.Models;
using JobPortal.Models.DTOs;

public class CompanyRegisterResponseMapper
{
    public CompanyRegisterResponseDto MapResponse(Company company)
    {
        return new CompanyRegisterResponseDto
        {
       
            CompanyName = company.CompanyName,
            Description  = company.Description,
            WebsiteUrl = company.WebsiteUrl,
            industryType = company.industryType,
            locations= company.locations
        };
    }
}
