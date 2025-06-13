using System.Security.Claims;
using JobPortal.Models;
using JobPortal.Models.DTOs;

namespace JobPortal.Interfaces;

public interface IRecruiterService
{
    Task<RecruiterRegisterResponseDto> RegisterCompany(RecruiterRegisterDto recruiterRegisterDto);
    Task<Recruiter> GetRecruiterById(Guid recruiterId);
    Task<IEnumerable<JobPostDto>> GetRecruiterJobPosts(Guid recruiterId);
    Task<Recruiter> GetRecruiterByUsername(string username);
    Task UpdateRecruiterDetails(Guid recruiterId, RecruiterUpdateDto updateDto);
    Task SoftDeleteRecruiter(Guid recruiterId, ClaimsPrincipal user);
}