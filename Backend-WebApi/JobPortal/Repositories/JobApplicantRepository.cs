

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
                                                        .Where(app => !app.IsDeleted)
                                                        .Include(app => app.seeker!).ThenInclude(seeker => seeker.seekerSkills!).ThenInclude(skill => skill.skill)
                                                        .Include(job => job.jobPost!).ThenInclude(j => j.requiredSkills!).ThenInclude(rs => rs.Skill)
                                                        .Include(job => job.jobPost!).ThenInclude(j => j.recruiter!).ThenInclude(c => c.company)
                                                        .FirstOrDefaultAsync(u => u.guid == key);
            if (application is null)
            {
                throw new RecordNotFoundException("Job Application with the given Id Not Found");
            }
            return application;
        }
        catch (RecordNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public override async Task<IEnumerable<JobApplication>> GetAll()
    {
        try
        {
            var applications = await _jobContext.jobApplications
                                                        .Where(app => !app.IsDeleted)
                                                        .Include(app => app.seeker!).ThenInclude(seeker => seeker.seekerSkills!).ThenInclude(skill => skill.skill)
                                                        .Include(job => job.jobPost!).ThenInclude(j => j.requiredSkills!).ThenInclude(rs => rs.Skill)
                                                        .Include(job => job.jobPost!).ThenInclude(j => j.recruiter!).ThenInclude(c => c.company)
                                                        .ToListAsync();

            return applications;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}