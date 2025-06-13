

using JobPortal.Contexts;
using Microsoft.EntityFrameworkCore;

public class JobPostPagedGet : IJobPostPagedGet
{
    private readonly JobContext _jobContext;

    public JobPostPagedGet(JobContext jobContext)
    {
        _jobContext = jobContext;
    }

    public async Task<IEnumerable<JobPost>> GetPaged(int pageNumber, int pageSize)
    {
        try
        {
            var posts = await _jobContext.jobPosts
                .Where(p => !p.IsDeleted)
                .Include(p => p.requiredSkills!)
                    .ThenInclude(ps => ps.Skill)
                .Include(p => p.responsibilities)
                .Include(p => p.requirements)
                .Include(p => p.recruiter)
                .OrderByDescending(p => p.PostedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return posts;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to fetch paged job posts", ex);
        }
    }

}