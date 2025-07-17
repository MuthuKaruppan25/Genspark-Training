

using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Repositories;

public class UserRepository : Repository<Guid, User>
{
    public UserRepository(JobContext context) : base(context)
    {

    }
    public override async Task<User> Get(Guid key)
    {
        try
        {
            var user = await _jobContext.users.FirstOrDefaultAsync(u => u.guid == key);
            if (user is null)
            {
                throw new RecordNotFoundException("User with the given Id Not Found");
            }
            return user;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public override async Task<IEnumerable<User>> GetAll()
    {
        try
        {
            var users = await _jobContext.users.ToListAsync();

            return users;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}