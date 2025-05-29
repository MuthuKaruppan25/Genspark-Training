


using BankApi.Models;
using Microsoft.EntityFrameworkCore;
using SecondWebApi.Repositories;

public class BranchRepository : Repository<string, Branch>
{
    public BranchRepository(BankContext bankContext) : base(bankContext)
    {

    }
    public override async Task<Branch> Get(string key)
    {
        var branch = await _bankContext.Branches.SingleOrDefaultAsync(u => u.IFSCCode == key);
        return branch ?? throw new Exception("No transations with given id found");
    }

    public override async Task<IEnumerable<Branch>> GetAll()
    {
        var branches= await _bankContext.Branches.ToListAsync();
        if (branches.Count() == 0)
        {
            throw new Exception("No users found");
        }
        return branches;
    }
}
