

using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Repositories;

public class JobPostRepository : Repository<Guid, JobPost>
{
    public JobPostRepository(JobContext context) : base(context)
    {

    }
    public override async Task<JobPost> Get(Guid key)
    {
        var post = await _jobContext.jobPosts
            .Where(p => !p.IsDeleted && p.guid == key)
            .Include(p => p.requiredSkills!).ThenInclude(rs => rs.Skill)
            .Include(p => p.responsibilities)
            .Include(p => p.locations)
            .Include(p => p.requirements)
            .Include(p => p.recruiter!).ThenInclude(r => r.company)
            .Include(j => j.jobApplications!)
                .ThenInclude(s => s.seeker!)
                    .ThenInclude(seeker => seeker.educations)
            .Include(j => j.jobApplications!)
                .ThenInclude(s => s.seeker!)
                    .ThenInclude(seeker => seeker.experiences)
            .Include(j => j.jobApplications!)
                .ThenInclude(s => s.seeker!)
                    .ThenInclude(seeker => seeker.seekerSkills!)
                        .ThenInclude(ss => ss.skill)
            .FirstOrDefaultAsync();

        if (post == null)
            throw new RecordNotFoundException("Job Post with the given Id Not Found");

        return post;
    }


    public override async Task<IEnumerable<JobPost>> GetAll()
    {
        try
        {
        var posts = await _jobContext.jobPosts
            .Where(p => !p.IsDeleted)
            .Include(p => p.requiredSkills!).ThenInclude(rs => rs.Skill)
            .Include(p => p.responsibilities)
            .Include(p => p.locations)
            .Include(p => p.requirements)
            .Include(p => p.recruiter!).ThenInclude(r => r.company)
            .Include(j => j.jobApplications!)
                .ThenInclude(s => s.seeker!)
                    .ThenInclude(seeker => seeker.educations)
            .Include(j => j.jobApplications!)
                .ThenInclude(s => s.seeker!)
                    .ThenInclude(seeker => seeker.experiences)
            .Include(j => j.jobApplications!)
                .ThenInclude(s => s.seeker!)
                    .ThenInclude(seeker => seeker.seekerSkills!)
                        .ThenInclude(ss => ss.skill)
            .ToListAsync();

            return posts;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}