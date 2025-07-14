

using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Repositories;

public class CompanyRepository : Repository<Guid, Company>
{
    public CompanyRepository(JobContext context) : base(context)
    {

    }
    public override async Task<Company> Get(Guid key)
    {
        try
        {
            var company = await _jobContext.companies
                                                .Where(c => !c.IsDeleted)
                                                .Include(r => r.recruiters)
                                                .Include(i => i.industryType)
                                                .Include(l => l.locations)
                                                .FirstOrDefaultAsync(u => u.guid == key);
            if (company is null)
            {
                throw new RecordNotFoundException("User with the given Id Not Found");
            }
            return company;
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

    public override async Task<IEnumerable<Company>> GetAll()
    {
        try
        {
            var companies = await _jobContext.companies
                                                .Where(c => !c.IsDeleted)
                                                .Include(r => r.recruiters)
                                                .Include(i => i.industryType)
                                                .Include(l => l.locations)
                                                .ToListAsync();
            return companies;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}