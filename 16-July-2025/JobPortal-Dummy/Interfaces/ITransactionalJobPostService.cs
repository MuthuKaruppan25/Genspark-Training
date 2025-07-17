public interface ITransactionalJobPostService
{
    Task<JobPostRegisterResponseDto> AddJobPostAsync(JobPostResponse jobPostDto);
}
