


using DocumentShare.Contexts;
using DocumentShare.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentShare.Repositories;
public class UserRepository : Repository<string, User>
{
    public UserRepository(FileContext context) : base(context)
    {

    }
    public override async Task<User> Get(string key)
    {
        var user = await _clinicContext.Users.SingleOrDefaultAsync(u => u.Username == key);
        if (user is null)
        {
            throw new Exception("No such user found");
        }
        return user;
    }

    public override async Task<IEnumerable<User>> GetAll()
    {
        return await _clinicContext.Users.ToListAsync();
    }

}