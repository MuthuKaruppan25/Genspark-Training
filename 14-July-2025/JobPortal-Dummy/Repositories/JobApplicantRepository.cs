using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Repositories;

public class JobApplicantRepository : Repository<Guid, JobApplication>
{
    public JobApplicantRepository(JobContext context) : base(context)
    {
    }

    public override async Task<JobApplication> Get(Guid key)
    {
        try
        {
            var application = await _jobContext.jobApplications
                .Where(app => !app.IsDeleted && app.guid == key)
                .Include(app => app.seeker!)
                    .ThenInclude(seeker => seeker.educations)
                .Include(app => app.seeker)
                    .ThenInclude(seeker => seeker.experiences)
                .Include(app => app.seeker)
                    .ThenInclude(seeker => seeker.seekerSkills)
                        .ThenInclude(ss => ss.skill)
                // .Include(app => app.jobPost)
                //     .ThenInclude(post => post.requiredSkills)
                //         .ThenInclude(rs => rs.Skill)
                // .Include(app => app.jobPost)
                //     .ThenInclude(post => post.recruiter)
                //         .ThenInclude(r => r.company)
                .FirstOrDefaultAsync();

            if (application is null)
                throw new RecordNotFoundException("Job Application with the given Id Not Found");

            return application;
        }
        catch (RecordNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving Job Application: {ex.Message}", ex);
        }
    }

    public override async Task<IEnumerable<JobApplication>> GetAll()
    {
        try
        {
            var applications = await _jobContext.jobApplications
                .Where(app => !app.IsDeleted)
                .Include(app => app.seeker)
                    .ThenInclude(seeker => seeker.educations)
                .Include(app => app.seeker)
                    .ThenInclude(seeker => seeker.experiences)
                .Include(app => app.seeker)
                    .ThenInclude(seeker => seeker.seekerSkills)
                        .ThenInclude(ss => ss.skill)
                // .Include(app => app.jobPost)
                //     .ThenInclude(post => post.requiredSkills)
                //         .ThenInclude(rs => rs.Skill)
                // .Include(app => app.jobPost)
                //     .ThenInclude(post => post.recruiter)
                //         .ThenInclude(r => r.company)
                .ToListAsync();

            return applications;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving all Job Applications: {ex.Message}", ex);
        }
    }
}
