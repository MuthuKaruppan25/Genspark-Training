public interface ITransactionalJobPostService
{
    Task<JobPostRegisterResponseDto> AddJobPostAsync(JobPostDto jobPostDto);
}
