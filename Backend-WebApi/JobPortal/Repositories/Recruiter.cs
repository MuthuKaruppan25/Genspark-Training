


using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Repositories;

public class RecruiterRepository : Repository<Guid, Recruiter>
{
    public RecruiterRepository(JobContext context) : base(context)
    {

    }
    public override async Task<Recruiter> Get(Guid key)
    {
        try
        {
            var recruiter = await _jobContext.recruiters
                                                    .Where(r => !r.IsDeleted)   
                                                    .Include(r => r.jobPosts)
                                                    .FirstOrDefaultAsync(u => u.guid == key);
            if (recruiter is null)
            {
                throw new RecordNotFoundException("Recruiter with the given Id Not Found");
            }
            return recruiter;
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

    public override async Task<IEnumerable<Recruiter>> GetAll()
    {
        try
        {
            var recruiters = await _jobContext.recruiters
                                                    .Where(r => !r.IsDeleted)   
                                                    .Include(r => r.jobPosts)
                                                    .ToListAsync();
            if (recruiters.Count() == 0)
            {
                throw new NoRecordsFoundException("No Recruiters Found");
            }
            return recruiters;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}