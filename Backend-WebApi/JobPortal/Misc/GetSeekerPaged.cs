using JobPortal.Contexts;
using Microsoft.EntityFrameworkCore;
using JobPortal.Models;

public class SeekerPagedGet : ISeekerPagedGet
{
    private readonly JobContext _context;

    public SeekerPagedGet(JobContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Seeker>> GetPaged(int pageNumber, int pageSize)
    {
        try
        {
            var seekers = await _context.seekers
                .Where(s => !s.IsDeleted)
                .Include(s => s.user)
                .Include(s => s.seekerSkills!)
                    .ThenInclude(ss => ss.skill)
                .OrderBy(s => s.FirstName) 
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return seekers;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to fetch paged seekers", ex);
        }
    }
}
