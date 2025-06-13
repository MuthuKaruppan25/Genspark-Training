using JobPortal.Models;
using JobPortal.Models.DTOs;

public class RecruiterMapper
{
    public Recruiter MapRecruiter(RecruiterRegisterDto dto, Guid userId, Guid companyId)
    {
        return new Recruiter
        {
            guid = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Designation = dto.Designation,
            UserId = userId,
            CompanyId = companyId,
            IsDeleted = false
        };
    }
}
