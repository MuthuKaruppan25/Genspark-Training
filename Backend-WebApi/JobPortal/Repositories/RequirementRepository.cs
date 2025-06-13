


using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Repositories;

public class RequirementRepository : Repository<Guid, Requirements>
{
    public RequirementRepository(JobContext context) : base(context)
    {

    }
    public override async Task<Requirements> Get(Guid key)
    {
        try
        {
            var requirement = await _jobContext.requirements.FirstOrDefaultAsync(u => u.guid == key);
            if (requirement is null)
            {
                throw new RecordNotFoundException("Requirement with the given Id Not Found");
            }
            return requirement;
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

    public override async Task<IEnumerable<Requirements>> GetAll()
    {
        try
        {
            var requirements = await _jobContext.requirements.ToListAsync();
            if (requirements.Count() == 0)
            {
                throw new NoRecordsFoundException("No Requirements Found");
            }
            return requirements;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}