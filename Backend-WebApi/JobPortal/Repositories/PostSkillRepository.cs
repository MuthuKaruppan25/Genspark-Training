


using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Repositories;

public class PostSkillsRepository : Repository<Guid, PostSkills>
{
    public PostSkillsRepository(JobContext context) : base(context)
    {

    }
    public override async Task<PostSkills> Get(Guid key)
    {
        try
        {
            var postSkill = await _jobContext.postSkills.FirstOrDefaultAsync(u => u.guid == key);
            if (postSkill is null)
            {
                throw new RecordNotFoundException("Post Skill with the given Id Not Found");
            }
            return postSkill;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public override async Task<IEnumerable<PostSkills>> GetAll()
    {
        try
        {
            var postSkills = await _jobContext.postSkills.ToListAsync();
            if (postSkills.Count() == 0)
            {
                throw new NoRecordsFoundException("No Job Posts Found");
            }
            return postSkills;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}