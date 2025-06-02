

using BankApi.Models;
using Microsoft.EntityFrameworkCore;
using SecondWebApi.Repositories;

public class UserRepository : Repository<string, User>
{
    public UserRepository(BankContext bankContext) : base(bankContext)
    {

    }
    public override async Task<User> Get(string key)
    {
        var users = await _bankContext.Users.SingleOrDefaultAsync(u => u.CustomerId == key);
        return users ?? throw new Exception("No users with given id found");
    }

    public override async Task<IEnumerable<User>> GetAll()
    {
        var users = await _bankContext.Users.ToListAsync();
        if (users.Count() == 0)
        {
            throw new Exception("No users found");
        }
        return users;
    }
}
