using JobPortal.Models;
using JobPortal.Models.DTOs;

public class JobPostMapper
{
    public JobPost Map(JobPostResponse dto)
    {
        return new JobPost
        {
            guid = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            EmploymentType = dto.EmploymentType,
            EmploymentPosition = dto.EmploymentPosition,
            Minsalary = dto.Minsalary,
            Maxsalary = dto.Maxsalary,
            LastDate = dto.LastDate,
            RecruiterID = dto.RecruiterId,
            PostedDate = DateTime.UtcNow,
            IsDeleted = false
        };
    }
}
