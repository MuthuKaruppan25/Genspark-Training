

using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Repositories;

public class IndustryTypeRepository : Repository<Guid, IndustryType>
{
    public IndustryTypeRepository(JobContext context) : base(context)
    {

    }
    public override async Task<IndustryType> Get(Guid key)
    {
        try
        {
            var Itype = await _jobContext.industryTypes.FirstOrDefaultAsync(u => u.guid == key);
            if (Itype is null)
            {
                throw new RecordNotFoundException("User with the given Id Not Found");
            }
            return Itype;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public override async Task<IEnumerable<IndustryType>> GetAll()
    {
        try
        {
            var ITypes = await _jobContext.industryTypes.ToListAsync();

            return ITypes;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}