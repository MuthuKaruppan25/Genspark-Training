

using JobPortal.Models;
using JobPortal.Models.DTOs;

public class IndustryTypeMapper
{
    public IndustryType? MapIndustryType(IndustryTypeRegister industryTypeRegister)
    {
        IndustryType industryType = new();
        industryType.guid = Guid.NewGuid();
        industryType.Name = industryTypeRegister!.Name;
        return industryType;
    }
}