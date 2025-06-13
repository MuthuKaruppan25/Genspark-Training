using System.Security.Claims;

namespace JobPortal.Interfaces;

public interface IJobPostService
{
    Task<JobPostRegisterResponseDto> AddJobPost(JobPostDto jobPostDto);
    Task<PagedResult<JobPostDto>> GetPagedJobPosts(PageDataDto pageDataDto);
    Task<PagedResult<JobPostDto>> GetJobPostsMatchingProfile(Guid SeekerId, int pageNumber, int pageSize);
    Task<IEnumerable<JobPostDto>> GetJobPostsByCompanyNameAsync(string companyName);
    Task<JobPostDto> GetJobPostByIdAsync(Guid postId);
    Task<JobPostWithApplicantsDto> GetJobPostWithPagedApplicants(Guid jobPostId, PageDataDto pageDataDto, ClaimsPrincipal user);
    Task<bool> SoftDeleteJobPost(Guid postId, ClaimsPrincipal claimsPrincipal);
    Task<JobPostDto> UpdateJobPost(Guid postId, JobPostUpdateDto updatedPostDto, ClaimsPrincipal user);
    Task UpdateResponsibilities(Guid postId, List<ResponsibilitiesAddDto> updatedResponsibilities, ClaimsPrincipal user);
    Task UpdateRequirements(Guid postId, List<RequirementsAddDto> updatedRequirements, ClaimsPrincipal user);
    Task UpdatePostSkills(Guid postId, List<SkillRegisterDto> updatedSkills, ClaimsPrincipal user);
    Task<IEnumerable<JobPostDto>> FilterJobPostsByTitle(string title);
    Task<IEnumerable<JobPostDto>> FilterJobPostsByLocation(string location);
    Task<IEnumerable<JobPostDto>> FilterJobPostsBySalary(decimal? minSalary = null, decimal? maxSalary = null);

    
}