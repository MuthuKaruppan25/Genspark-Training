using JobPortal.Models;
using JobPortal.Models.DTOs;

namespace JobPortal.Interfaces;

public interface ICompanyService
{
    Task<CompanyRegisterResponseDto> RegisterCompany(CompanyRegisterDto companyRegisterDto);
    Task<IEnumerable<Recruiter>> GetRecruitersInCompany(Guid companyId);
    Task<IEnumerable<string>> GetAllCompanies();
    Task<IEnumerable<Address>> GetCompanyLocations(Guid companyId);
    Task<Company> GetCompanyByRecruiterId(Guid recruiterId);
    Task UpdateCompanyDetails(Guid companyId, CompanyUpdateDto updateDto);
    Task UpdateCompanyLocations(Guid companyId, List<AddressRegisterDto> locationDtos);
    Task<Company> SoftDeleteCompany(Guid companyId);
    Task<IEnumerable<Company>> Companies();
}