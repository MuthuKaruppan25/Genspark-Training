public class JobPostResMapper
{
    public JobPostDto MapToDto(JobPost post)
    {
        return new JobPostDto
        {
            id = post.guid,
            Title = post.Title,
            Description = post.Description,
            EmploymentType = post.EmploymentType,
            EmploymentPosition = post.EmploymentPosition,
            locations = post.locations?
                        .Select(l => new LocationDto { Name = l.Name })
                        .ToList(),
            Minsalary = post.Minsalary,
            Maxsalary = post.Maxsalary,
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
    public JobPostDetailsDto MapDetailsToDto(JobPost post)
    {
        return new JobPostDetailsDto
        {
            id = post.guid,
            Title = post.Title,
            Description = post.Description,
            EmploymentType = post.EmploymentType,
            EmploymentPosition = post.EmploymentPosition,
            locations = post.locations?
                        .Select(l => new LocationDto { Name = l.Name })
                        .ToList(),
            PostedDate = post.PostedDate,
            Minsalary = post.Minsalary,
            Maxsalary = post.Maxsalary,
            RecruiterId = post.RecruiterID,
            LastDate = post.LastDate,
            requirements = post.requirements?
                .Select(r => new RequirementsAddDto { Name = r.Name })
                .ToList(),
            
            jobApplications = post.jobApplications?
                              .Select(j => new ApplicationsDto{guid=j.guid,AppliedOn = j.AppliedOn,Status = j.Status})
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
