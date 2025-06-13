using JobPortal.Models;

public class JobApplicationDetailsMapper
{
    public JobApplicationDetailsDto Map(JobApplication app)
    {
        return new JobApplicationDetailsDto
        {
            ApplicationId = app.guid,
            AppliedOn = app.AppliedOn,
            Status = app.Status,
            SeekerId = app.SeekerId,
            SeekerName = $"{app.seeker?.FirstName}{app.seeker?.LastName}" ?? "Unknown",
            SeekerSkills = app.seeker?.seekerSkills?
                                .Select(s => s.skill?.Name ?? "Unnamed Skill")
                                .ToList() ?? new(),

            JobPostId = app.JobPostId,
            JobPostTitle = app.jobPost?.Title ?? "Unknown",
            RequiredSkills = app.jobPost?.requiredSkills?
                                .Select(rs => rs.Skill?.Name ?? "Unnamed Skill")
                                .ToList() ?? new(),

            RecruiterName = app.jobPost?.recruiter != null
                            ? $"{app.jobPost.recruiter.FirstName ?? ""} {app.jobPost.recruiter.LastName ?? ""}".Trim()
                            : "Unknown",
            CompanyName = app.jobPost?.recruiter?.company?.CompanyName ?? "Unknown"
        };
    }

    public List<JobApplicationDetailsDto> MapList(IEnumerable<JobApplication> apps)
    {
        return apps.Select(Map).ToList();
    }
}
