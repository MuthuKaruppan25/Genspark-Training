

using BankApi.Models;
using Microsoft.EntityFrameworkCore;
using SecondWebApi.Repositories;

public class AccountRepository : Repository<string, Account>
{
    public AccountRepository(BankContext bankContext) : base(bankContext)
    {

    }
    public override async Task<Account> Get(string key)
    {
        var account = await _bankContext.Accounts
                                        .Include(u => u.user)
                                        .Include(b => b.Branch)
                                        .Include(t => t.Transactions)
                                        .SingleOrDefaultAsync(u => u.AccountNo == key);
        return account ?? throw new Exception("No accounts with given id found");
    }

    public override async Task<IEnumerable<Account>> GetAll()
    {
        var accounts = await _bankContext.Accounts
                                        .Include(u => u.user)
                                        .Include(b => b.Branch)
                                        .Include(t => t.Transactions)
                                        .ToListAsync();
        if (accounts.Count() == 0)
        {
            throw new Exception("No users found");
        }
        return accounts;
    }
}
