
public interface IJobPostPagedGet
{
    Task<IEnumerable<JobPost>> GetPaged(int pageNumber, int pageSize);
}