using JobPortal.Contexts;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

public class JobApplicationPagedGet : IJobApplicationPaged
{
    private readonly JobContext _jobContext;

    public JobApplicationPagedGet(JobContext jobContext)
    {
        _jobContext = jobContext;
    }

    public async Task<IEnumerable<JobApplication>> GetPaged(int pageNumber, int pageSize)
    {
        try
        {
            return await _jobContext.jobApplications
                .Where(app => !app.IsDeleted)
                .Include(app => app.seeker!).ThenInclude(seeker => seeker.seekerSkills!).ThenInclude(skill => skill.skill)
                .Include(job => job.jobPost!).ThenInclude(j => j.requiredSkills!).ThenInclude(rs => rs.Skill)
                .Include(job => job.jobPost!).ThenInclude(j => j.recruiter!).ThenInclude(c => c.company)
                .OrderByDescending(app => app.AppliedOn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

           
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to fetch paged job applications", ex);
        }
    }
}
