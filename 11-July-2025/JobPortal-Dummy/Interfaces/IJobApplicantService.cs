using System.Security.Claims;
using JobPortal.Models;
namespace JobPortal.Interfaces;

public interface IJobApplicantService
{
    Task<JobApplicantAddResponse> CreateApplication(JobApplicantAddDto jobApplicantAddDto, ClaimsPrincipal userPrincipal);
    Task<PagedResult<JobApplicationDetailsDto>> GetPagedApplications(int pageNumber, int pageSize);
    Task<bool> SoftDeleteApplication(Guid applicationId, ClaimsPrincipal user);
    Task<IEnumerable<JobApplication>> GetApplicationsByPostId(Guid postId);
    Task<JobApplication> UpdateStatus(Guid appId, StatusUpdate statusUpdate);
    Task<bool> CheckIfApplicationExists(Guid postId, Guid seekerId);
    
}