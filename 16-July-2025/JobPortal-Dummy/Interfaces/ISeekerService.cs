using System.Security.Claims;
using JobPortal.Models;
using JobPortal.Models.DTOs;
namespace JobPortal.Interfaces;

public interface ISeekerService
{
    Task<SeekerRegisterResponseDto> RegisterSeeker(SeekerRegisterDto seekerRegisterDto);
    Task<IEnumerable<SeekerRegisterResponseDto>> GetPagedSeekers(int pageNumber, int pageSize);
    Task<SeekerWithApplicationsDto> GetSeekerWithApplications(string username);
    Task<List<SkillRegisterDto>> GetSeekerSkills(string username);
    Task UpdateSeekerDetails(string username, SeekerUpdateDto dt, ClaimsPrincipal userPrincipal);
    Task UpdateSeekerExperiences(string username, SeekerExperienceUpdateDto dto, ClaimsPrincipal userPrincipal);
    Task UpdateSeekerEducations(string username, SeekerEducationUpdateDto dto, ClaimsPrincipal userPrincipal);
    Task UpdateSeekerSkills(string username, List<SkillRegisterDto> skillNames, ClaimsPrincipal userPrincipal);
    Task<Seeker> GetSeekerByUsername(string username);
    Task SoftDeleteSeekerAsync(string username, ClaimsPrincipal userPrincipal);
    Task<IEnumerable<SeekerRegisterResponseDto>> SearchSeekersByName(string name);
    Task<IEnumerable<SeekerRegisterResponseDto>> SearchSeekersBySkills(List<SkillRegisterDto> skillDtos);
    Task<IEnumerable<SeekerRegisterResponseDto>> SearchSeekersByEducation(string education);

}