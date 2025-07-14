using JobPortal.Models;

public interface ISeekerPagedGet
{
    Task<IEnumerable<Seeker>> GetPaged(int pageNumber, int pageSize);
}
