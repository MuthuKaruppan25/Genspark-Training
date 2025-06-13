
using JobPortal.Models;

public interface IJobApplicationPaged
{
    Task<IEnumerable<JobApplication>> GetPaged(int pageNumber, int pageSize);
}