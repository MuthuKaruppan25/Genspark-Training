
using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Repositories;
using Microsoft.EntityFrameworkCore;

public class EducationRepository : Repository<Guid, Education>
{
    public EducationRepository(JobContext context) : base(context)
    {


    }

    public override async Task<Education> Get(Guid key)
    {
        var experience = await _jobContext.educations.FirstOrDefaultAsync(u => u.guid == key);
        if (experience is null)
            throw new RecordNotFoundException("Experience is not found");
        return experience;
    }

    public override async Task<IEnumerable<Education>> GetAll()
    {
        var experiences = await _jobContext.educations.ToListAsync();
        return experiences;
    }
}