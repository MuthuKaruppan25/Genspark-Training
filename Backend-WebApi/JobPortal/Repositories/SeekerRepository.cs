



using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Repositories;

public class SeekerRepository : Repository<Guid, Seeker>
{
    public SeekerRepository(JobContext context) : base(context)
    {

    }
    public override async Task<Seeker> Get(Guid key)
    {
        try
        {
            var seeker = await _jobContext.seekers
                                                .Where(s => !s.IsDeleted && s.guid == key)
                                                .Include(s => s.seekerSkills!).ThenInclude(ss => ss.skill)
                                                .Include(s => s.jobApplications!).ThenInclude(p => p.jobPost)
                                                .FirstOrDefaultAsync();
            if (seeker is null)
            {
                throw new RecordNotFoundException("Seeker with the given Id Not Found");
            }
            return seeker;
        }
        catch (RecordNotFoundException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public override async Task<IEnumerable<Seeker>> GetAll()
    {
        try
        {
            var seekers = await _jobContext.seekers
                                                .Where(s => !s.IsDeleted)
                                                .Include(s => s.seekerSkills!).ThenInclude(ss => ss.skill)
                                                .Include(s => s.jobApplications!).ThenInclude(p => p.jobPost)
                                                .ToListAsync();

            return seekers;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}