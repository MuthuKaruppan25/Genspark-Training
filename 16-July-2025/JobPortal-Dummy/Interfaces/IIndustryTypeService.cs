using JobPortal.Models;

namespace JobPortal.Interfaces;

public interface IIndustryTypeService
{
    Task<IndustryType> AddIndustryType(IndustryTypeRegister industryTypeRegister);
    Task UpdateIndustryType(Guid industryTypeId, IndustryTypeRegister industryTypeRegister);
}