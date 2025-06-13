public class JobPostResMapper
{
    public JobPostDto MapToDto(JobPost post)
    {
        return new JobPostDto
        {
            Title = post.Title,
            Description = post.Description,
            EmploymentType = post.EmploymentType,
            EmploymentPosition = post.EmploymentPosition,
            Location = post.Location,
            SalaryPackage = post.SalaryPackage,
            RecruiterId = post.RecruiterID,
            LastDate = post.LastDate,
            requirements = post.requirements?
                .Select(r => new RequirementsAddDto { Name = r.Name })
                .ToList(),

            responsibilities = post.responsibilities?
                
                .Select(r => new ResponsibilitiesAddDto { Name = r.Name })
                .ToList(),

            skills = post.requiredSkills?
                .Where(s => s.Skill != null)
                .Select(s => new SkillRegisterDto { Name = s.Skill.Name })
                .ToList()
        };
    }
}
