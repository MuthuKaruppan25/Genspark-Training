
using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Repositories;
using Microsoft.EntityFrameworkCore;

public class ExperienceRepository : Repository<Guid, Experience>
{
    public ExperienceRepository(JobContext context) : base(context)
    {


    }

    public override async Task<Experience> Get(Guid key)
    {
        var experience = await _jobContext.experiences.FirstOrDefaultAsync(u => u.Guid == key);
        if (experience is null)
            throw new RecordNotFoundException("Experience is not found");
        return experience;
    }

    public override async Task<IEnumerable<Experience>> GetAll()
    {
        var experiences = await _jobContext.experiences.ToListAsync();
        return experiences;
    }
}