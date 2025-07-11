



using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Repositories;

public class SeekerSkillsRepository : Repository<Guid, SeekerSkills>
{
    public SeekerSkillsRepository(JobContext context) : base(context)
    {

    }
    public override async Task<SeekerSkills> Get(Guid key)
    {
        try
        {
            var seeker = await _jobContext.seekerSkills.FirstOrDefaultAsync(u => u.guid == key);
            if (seeker is null)
            {
                throw new RecordNotFoundException("Seeker Skill with the given Id Not Found");
            }
            return seeker;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public override async Task<IEnumerable<SeekerSkills>> GetAll()
    {
        try
        {
            var seekerSkills = await _jobContext.seekerSkills.ToListAsync();
            if (seekerSkills.Count() == 0)
            {
                throw new NoRecordsFoundException("No Seeker Skills Found");
            }
            return seekerSkills;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}