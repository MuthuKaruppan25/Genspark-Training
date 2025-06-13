


using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Repositories;

public class ResponsibilityRepository : Repository<Guid, Responsibilities>
{
    public ResponsibilityRepository(JobContext context) : base(context)
    {

    }
    public override async Task<Responsibilities> Get(Guid key)
    {
        try
        {
            var responsibility = await _jobContext.responsibilities.FirstOrDefaultAsync(u => u.guid == key);
            if (responsibility is null)
            {
                throw new RecordNotFoundException("Responsibility with the given Id Not Found");
            }
            return responsibility;
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

    public override async Task<IEnumerable<Responsibilities>> GetAll()
    {
        try
        {
            var responsibilities = await _jobContext.responsibilities.ToListAsync();
            if (responsibilities.Count() == 0)
            {
                throw new NoRecordsFoundException("No responsibilities Found");
            }
            return responsibilities;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}