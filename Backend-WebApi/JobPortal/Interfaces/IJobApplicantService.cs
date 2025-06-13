using System.Security.Claims;

namespace JobPortal.Interfaces;
public interface IJobApplicantService
{
    Task<JobApplicantAddResponse> CreateApplication(JobApplicantAddDto jobApplicantAddDto,ClaimsPrincipal userPrincipal);
    Task<PagedResult<JobApplicationDetailsDto>> GetPagedApplications(int pageNumber, int pageSize);
    Task<bool> SoftDeleteApplication(Guid applicationId,ClaimsPrincipal user);
}