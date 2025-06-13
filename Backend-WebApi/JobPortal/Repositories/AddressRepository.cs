

using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Repositories;

public class AddressRepository : Repository<Guid, Address>
{
    public AddressRepository(JobContext context) : base(context)
    {

    }
    public override async Task<Address> Get(Guid key)
    {
        try
        {
            var location = await _jobContext.address.FirstOrDefaultAsync(u => u.guid == key);
            if (location is null)
            {
                throw new RecordNotFoundException("User with the given Id Not Found");
            }
            return location;
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

    public override async Task<IEnumerable<Address>> GetAll()
    {
        try
        {
            var locations = await _jobContext.address.ToListAsync();
            if (locations.Count() == 0)
            {
                throw new NoRecordsFoundException("No Users Found");
            }
            return locations;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}