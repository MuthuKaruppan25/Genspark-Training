



using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Repositories;

public class SkillsRepository : Repository<Guid, Skill>
{
    public SkillsRepository(JobContext context) : base(context)
    {

    }
    public override async Task<Skill> Get(Guid key)
    {
        try
        {
            var skills = await _jobContext.skills.FirstOrDefaultAsync(u => u.guid == key);
            if (skills is null)
            {
                throw new RecordNotFoundException("Skill with the given Id Not Found");
            }
            return skills;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public override async Task<IEnumerable<Skill>> GetAll()
    {
        try
        {
            var skills = await _jobContext.skills.ToListAsync();

            return skills;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}