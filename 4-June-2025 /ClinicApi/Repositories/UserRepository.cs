


using Microsoft.EntityFrameworkCore;
using SecondWebApi.Contexts;


namespace SecondWebApi.Repositories;
public class UserRepository : Repository<string, User>
{
    public UserRepository(ClinicContext context) : base(context)
    {

    }
    public override async Task<User> Get(string key)
    {
        return await _clinicContext.users.SingleOrDefaultAsync(u => u.username == key);
    }

    public override async Task<IEnumerable<User>> GetAll()
    {
        return await _clinicContext.users.ToListAsync();
    }

}