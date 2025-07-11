using System.Security.Claims;
using JobPortal.Models;

namespace JobPortal.Interfaces;

public interface IJobPostService
{
    Task<JobPostRegisterResponseDto> AddJobPost(JobPostResponse jobPostDto);
    Task<PagedResult<JobPostDto>> GetPagedJobPosts(PageDataDto pageDataDto);
    Task<PagedResult<JobPostDto>> GetJobPostsMatchingProfile(Guid SeekerId, int pageNumber, int pageSize);
    Task<IEnumerable<JobPostDto>> GetJobPostsByCompanyNameAsync(string companyName);
    Task<JobPostDto> GetJobPostByIdAsync(Guid postId);
    // Task<JobPostWithApplicantsDto> GetJobPostWithPagedApplicants(Guid jobPostId, PageDataDto pageDataDto, ClaimsPrincipal user);
    Task<PagedResult<JobApplication>> GetJobApplicationsByPost(Guid jobPostId, PageDataDto pageDataDto, ClaimsPrincipal user);
    Task<bool> SoftDeleteJobPost(Guid postId, ClaimsPrincipal claimsPrincipal);
    Task<JobPostDto> UpdateJobPost(Guid postId, JobPostUpdateDto updatedPostDto, ClaimsPrincipal user);
    Task UpdateResponsibilities(Guid postId, ResponsibilitiesUpdateDto updatedResponsibilities, ClaimsPrincipal user);
    Task UpdateRequirements(Guid postId, RequirementsUpdateDto updatedRequirements, ClaimsPrincipal user);
    Task UpdatePostSkills(Guid postId, SkillsUpdateDto updatedSkills, ClaimsPrincipal user);
    Task<IEnumerable<JobPostDto>> FilterJobPostsByTitle(string title);
    Task<IEnumerable<JobPostDto>> FilterJobPostsByLocation(string location);
    Task<IEnumerable<JobPostDto>> FilterJobPostsBySalary(int? minSalary = null, int? maxSalary = null);

    
}