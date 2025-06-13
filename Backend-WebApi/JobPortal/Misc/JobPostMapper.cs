using JobPortal.Models;
using JobPortal.Models.DTOs;

public class JobPostMapper
{
    public JobPost Map(JobPostDto dto)
    {
        return new JobPost
        {
            guid = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            EmploymentType = dto.EmploymentType,
            EmploymentPosition = dto.EmploymentPosition,
            Location = dto.Location,
            SalaryPackage = dto.SalaryPackage,
            LastDate = dto.LastDate,
            RecruiterID = dto.RecruiterId,
            PostedDate = DateTime.UtcNow,
            IsDeleted = false
        };
    }
}
