using System.Security.Claims;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using log4net;
using Microsoft.AspNetCore.SignalR;

public class JobPostService : IJobPostService
{
    private static readonly ILog _errorLogger = LogManager.GetLogger("ErrorFileAppender");
    private static readonly ILog _dataLogger = LogManager.GetLogger("DataChangeFileAppender");

    private readonly IRepository<Guid, JobPost> _jobPostRepository;
    private readonly IRepository<Guid, Recruiter> _recruiterRepository;
    private readonly IRepository<Guid, Skill> _skillRepository;
    private readonly IRepository<Guid, Requirements> _requirementRepository;
    private readonly IRepository<Guid, Responsibilities> _responsibilityRepository;
    private readonly IRepository<Guid, PostSkills> _postSkillRepository;
    private readonly IJobPostPagedGet _jobPostPagedGet;
    private readonly IRepository<Guid, User> _userRepository;
    private readonly IRepository<Guid, Seeker> _seekerRepository;
    private readonly ITransactionalJobPostService _transactionalJobPostService;
    private readonly ISeekerNotificationService _seekerNotificationService;
    private readonly JobPostMapper _jobPostMapper;
    private readonly RequirementMapper _requirementMapper;
    private readonly ResponsibilityMapper _responsibilityMapper;
    private readonly PostSkillMapper _postSkillMapper;
    private readonly JobPostResponseMapper _responseMapper;
    private readonly JobPostResMapper jobPostResMapper;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly JobPostWithApplicantsMapper jobPostWithApplicantsMapper;

    public JobPostService(
        IRepository<Guid, JobPost> jobPostRepository,
        IRepository<Guid, Recruiter> recruiterRepository,
        IRepository<Guid, Skill> skillRepository,
        IRepository<Guid, Requirements> requirementRepository,
        IRepository<Guid, Responsibilities> responsibilityRepository,
        IRepository<Guid, User> userRepository,
        IRepository<Guid, Seeker> seekerRepository,
        IHubContext<NotificationHub> hubContext,
        ITransactionalJobPostService transactionalJobPostService,
        ISeekerNotificationService seekerNotificationService,
        IJobPostPagedGet jobPostPagedGet,
        IRepository<Guid, PostSkills> postSkillRepository)
    {
        _jobPostRepository = jobPostRepository;
        _recruiterRepository = recruiterRepository;
        _skillRepository = skillRepository;
        _requirementRepository = requirementRepository;
        _responsibilityRepository = responsibilityRepository;
        _postSkillRepository = postSkillRepository;
        _jobPostPagedGet = jobPostPagedGet;
        _userRepository = userRepository;
        _seekerRepository = seekerRepository;
        _transactionalJobPostService = transactionalJobPostService;
        _seekerNotificationService = seekerNotificationService;
        _hubContext = hubContext;
        _jobPostMapper = new JobPostMapper();
        _requirementMapper = new RequirementMapper();
        jobPostWithApplicantsMapper = new JobPostWithApplicantsMapper();
        _responsibilityMapper = new ResponsibilityMapper();
        _postSkillMapper = new PostSkillMapper();
        _responseMapper = new JobPostResponseMapper();
        jobPostResMapper = new JobPostResMapper();
    }

    public async Task<JobPostRegisterResponseDto> AddJobPost(JobPostDto jobPostDto)
    {
        try
        {
            var result = await _transactionalJobPostService.AddJobPostAsync(jobPostDto);
            await _seekerNotificationService.NotifySeekersAsync(result.Title, result.LastDate);
            _dataLogger.Info($"JobPost added by recruiter: {jobPostDto.RecruiterId}, Job Title: {jobPostDto.Title}");
            return result;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error($"Recruiter not found: {jobPostDto.RecruiterId}", ex);
            throw;
        }
        catch (DuplicateEntryException ex)
        {
            _errorLogger.Error("Field required exception during AddJobPost", ex);
            throw;
        }
        catch (FieldRequiredException ex)
        {
            _errorLogger.Error("Field required exception during AddJobPost", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Unexpected error in AddJobPost", ex);
            throw new RegistrationException("Failed to add job post", ex);
        }
    }

    public async Task<PagedResult<JobPostDto>> GetPagedJobPosts(PageDataDto pageDataDto)
    {
        try
        {
            var posts = await _jobPostPagedGet.GetPaged(pageDataDto.pageNumber, pageDataDto.pageSize);
            var totalCount = posts.Count();
            var resultDtos = posts.OrderByDescending(p => p.PostedDate).Select(p => jobPostResMapper.MapToDto(p)).ToList();

            return new PagedResult<JobPostDto>
            {
                Items = resultDtos,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Error while fetching paged job posts", ex);
            throw new FetchDataException(ex.Message);
        }
    }

    public async Task<PagedResult<JobPostDto>> GetJobPostsMatchingProfile(Guid SeekerID, int pageNumber, int pageSize)
    {
        try
        {

            var seeker = await _seekerRepository.Get(SeekerID);
            var seekerSkillIds = seeker.seekerSkills?.Select(ss => ss.SkillId).ToHashSet() ?? new();

            var allPosts = await _jobPostRepository.GetAll();
            var scoredPosts = allPosts
                .Where(p => p.requiredSkills != null)
                .Select(p =>
                {
                    var requiredSkillIds = p.requiredSkills!.Select(rs => rs.SkillId).ToHashSet();
                    int matchCount = requiredSkillIds.Intersect(seekerSkillIds).Count();
                    return new { JobPost = p, MatchScore = matchCount };
                })
                .Where(x => x.MatchScore > 0)
                .OrderByDescending(x => x.MatchScore)
                .ToList();

            var totalCount = scoredPosts.Count;

            var pagedDtos = scoredPosts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => jobPostResMapper.MapToDto(x.JobPost))
                .ToList();

            return new PagedResult<JobPostDto>
            {
                Items = pagedDtos,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _errorLogger.Error($"Error while matching profile for user: {SeekerID}", ex);
            throw new FetchDataException(ex.Message);
        }
    }


    public async Task<IEnumerable<JobPostDto>> GetJobPostsByCompanyNameAsync(string companyName)
    {
        try
        {
            var allPosts = await _jobPostRepository.GetAll();

            var companyPosts = allPosts
                .Where(p =>
                    !p.IsDeleted &&
                    p.recruiter?.company?.CompanyName.Equals(companyName, StringComparison.OrdinalIgnoreCase) == true)
                .OrderByDescending(p => p.PostedDate)
                .Select(p => jobPostResMapper.MapToDto(p))
                .ToList();

            return companyPosts;
        }
        catch (Exception ex)
        {
            _errorLogger.Error($"Failed to fetch job posts for company: {companyName}", ex);
            throw new FetchDataException($"Failed to fetch job posts for company name '{companyName}'", ex);
        }
    }

    public async Task<JobPostDto> GetJobPostByIdAsync(Guid postId)
    {
        try
        {
            var post = await _jobPostRepository.Get(postId);
            return jobPostResMapper.MapToDto(post);
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error($"Job post not found with id: {postId}", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error($"Failed to fetch job post by ID {postId}", ex);
            throw new FetchDataException($"Failed to fetch job post by ID {postId}", ex);
        }
    }

    public async Task<JobPostWithApplicantsDto> GetJobPostWithPagedApplicants(Guid jobPostId, PageDataDto pageDataDto, ClaimsPrincipal user)
    {
        try
        {
            var username = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var users = await _userRepository.GetAll();
            var userId = users.FirstOrDefault(u => u.Username == username)?.guid;
            if (userId == null)
                throw new UnauthorizedAccessException("User not found.");
            var recruiters = await _recruiterRepository.GetAll();
            var recruiter = recruiters.FirstOrDefault(r => r.UserId == userId && !r.IsDeleted);
            if (recruiter == null)
                throw new UnauthorizedAccessException("You are not authorized to view this job post.");
            var post = await _jobPostRepository.Get(jobPostId)
                       ?? throw new RecordNotFoundException("Job Post with the given Id Not Found");
            if (!post.IsDeleted && post.RecruiterID != recruiter.guid)
                throw new UnauthorizedAccessException("You are not authorized to view this job post.");
            var requiredSkillIds = post.requiredSkills?.Select(rs => rs.SkillId).ToHashSet() ?? new();
            var requiredSkillNames = post.requiredSkills?.Select(rs => rs.Skill?.Name).Where(n => !string.IsNullOrEmpty(n)).ToList() ?? new List<string>();
            var applications = post.jobApplications ?? new List<JobApplication>();

            var scoredApplicants = applications
                .Where(app => !app.IsDeleted && app.seeker != null)
                .Select(app =>
                {
                    var applicantSkillIds = app.seeker!.seekerSkills?.Select(ss => ss.SkillId).ToHashSet() ?? new();
                    int matchScore = requiredSkillIds.Intersect(applicantSkillIds).Count();

                    // Get seeker name and skills
                    var seekerName = app.seeker?.FirstName ?? string.Empty;
                    var seekerSkills = app.seeker?.seekerSkills?.Select(ss => ss.skill?.Name).Where(n => !string.IsNullOrEmpty(n)).ToList() ?? new List<string>();

                    return new
                    {
                        Applicant = app,
                        MatchScore = matchScore,
                        SeekerName = seekerName,
                        SeekerSkills = seekerSkills
                    };
                })
                .OrderByDescending(x => x.MatchScore)
                .ToList();

            var totalCount = scoredApplicants.Count;

            var pagedApplicants = scoredApplicants
                .Skip((pageDataDto.pageNumber - 1) * pageDataDto.pageSize)
                .Take(pageDataDto.pageSize)
                .Select(x => new JobApplicantAddResponse
                {
                    SeekerId = x.Applicant.SeekerId,
                    JobPostId = x.Applicant.JobPostId,
                    AppliedOn = x.Applicant.AppliedOn,
                    Status = x.Applicant.Status,
                    SeekerName = x.SeekerName,
                    SeekerSkills = x.SeekerSkills!
                })
                .ToList();

            var dto = new JobPostWithApplicantsDto
            {
                JobPostId = post.guid,
                Title = post.Title,
                Description = post.Description,
                RequiredSkills = requiredSkillNames,
                Applicants = new PagedResult<JobApplicantAddResponse>
                {
                    Items = pagedApplicants,
                    TotalCount = totalCount
                }
            };

            return dto;
        }
        catch (Exception ex)
        {
            _errorLogger.Error($"Error in GetJobPostWithPagedApplicants for postId {jobPostId}", ex);
            throw new FetchDataException($"Failed to fetch job post with applicants: {ex.Message}");
        }
    }

    public async Task<bool> SoftDeleteJobPost(Guid postId, ClaimsPrincipal user)
    {
        try
        {
            var post = await _jobPostRepository.Get(postId);
            if (post == null || post.IsDeleted)
                throw new RecordNotFoundException("Job post not found");


            var username = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("User ID not found in token.");

            var users = await _userRepository.GetAll();
            var userId = users.FirstOrDefault(u => u.Username == username)?.guid;
            if (userId == null)
                throw new UnauthorizedAccessException("User not found.");
            var recruiters = await _recruiterRepository.GetAll();

            var recruiter = recruiters.FirstOrDefault(r => r.UserId == userId && !r.IsDeleted);
            if (recruiter == null)
                throw new UnauthorizedAccessException("Recruiter not found for current user.");


            if (post.RecruiterID != recruiter.guid)
                throw new UnauthorizedAccessException("You are not authorized to delete this job post.");

            post.IsDeleted = true;
            await _jobPostRepository.Update(postId, post);

            _dataLogger.Info($"Soft deleted job post: {postId}, RecruiterId: {recruiter.guid}");
            return true;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error($"Soft delete failed: job post not found {postId}", ex);
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            _errorLogger.Error($"Unauthorized delete attempt for job post {postId}", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error($"Error during soft delete for job post: {postId}", ex);
            throw new UpdateException($"Failed to soft delete job post: {ex.Message}");
        }
    }


    public async Task<JobPostDto> UpdateJobPost(Guid postId, JobPostUpdateDto updatedPostDto, ClaimsPrincipal user)
    {
        try
        {
            var existingPost = await _jobPostRepository.Get(postId);
            if (existingPost == null || existingPost.IsDeleted)
                throw new RecordNotFoundException("Job post not found");

            // Authorization logic
            var username = user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("User ID not found in token.");

            var users = await _userRepository.GetAll();
            var currentUser = users.FirstOrDefault(u => u.Username == username);
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not found.");

            var recruiters = await _recruiterRepository.GetAll();
            var recruiter = recruiters.FirstOrDefault(r => r.UserId == currentUser.guid && !r.IsDeleted);
            if (recruiter == null)
                throw new UnauthorizedAccessException("Recruiter not found for current user.");

            if (existingPost.RecruiterID != recruiter.guid)
                throw new UnauthorizedAccessException("You are not authorized to update this job post.");

            existingPost.Title = updatedPostDto.Title;
            existingPost.Description = updatedPostDto.Description;
            existingPost.Location = updatedPostDto.Location;
            existingPost.SalaryPackage = updatedPostDto.SalaryPackage;
            existingPost.EmploymentType = updatedPostDto.EmploymentType;
            existingPost.EmploymentPosition = updatedPostDto.EmploymentPosition;
            existingPost.LastDate = updatedPostDto.ClosingDate;
            existingPost.PostedDate = DateTime.UtcNow;

            await _jobPostRepository.Update(postId, existingPost);
            _dataLogger.Info($"Updated job post: {postId}");

            return jobPostResMapper.MapToDto(existingPost);
        }
        catch (Exception ex)
        {
            _errorLogger.Error($"Failed to update job post: {postId}", ex);
            throw new UpdateException($"Failed to update job post: {ex.Message}");
        }
    }

    public async Task UpdateResponsibilities(Guid postId, List<ResponsibilitiesAddDto> updatedResponsibilities, ClaimsPrincipal user)
    {
        try
        {
            var post = await _jobPostRepository.Get(postId) ?? throw new RecordNotFoundException("Job post not found");


            var username = user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("User ID not found in token.");

            var users = await _userRepository.GetAll();
            var currentUser = users.FirstOrDefault(u => u.Username == username);
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not found.");

            var recruiters = await _recruiterRepository.GetAll();
            var recruiter = recruiters.FirstOrDefault(r => r.UserId == currentUser.guid && !r.IsDeleted);
            if (recruiter == null)
                throw new UnauthorizedAccessException("Recruiter not found for current user.");

            if (post.RecruiterID != recruiter.guid)
                throw new UnauthorizedAccessException("You are not authorized to update this job post.");

            var existingResponsibilities = post.responsibilities?.ToList() ?? new();
            foreach (var old in existingResponsibilities)
                await _responsibilityRepository.Delete(old.guid);

            var newEntities = _responsibilityMapper.MapList(updatedResponsibilities, postId);
            foreach (var responsibility in newEntities)
                await _responsibilityRepository.Add(responsibility);

            _dataLogger.Info($"Updated responsibilities for postId: {postId}");
        }
        catch (Exception ex)
        {
            _errorLogger.Error($"Error updating responsibilities for job post: {postId}", ex);
            throw new UpdateException($"Failed to update responsibilities: {ex.Message}");
        }
    }

    public async Task UpdateRequirements(Guid postId, List<RequirementsAddDto> updatedRequirements, ClaimsPrincipal user)
    {
        try
        {
            var post = await _jobPostRepository.Get(postId) ?? throw new RecordNotFoundException("Job post not found");

            // Authorization logic
            var username = user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("User ID not found in token.");

            var users = await _userRepository.GetAll();
            var currentUser = users.FirstOrDefault(u => u.Username == username);
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not found.");

            var recruiters = await _recruiterRepository.GetAll();
            var recruiter = recruiters.FirstOrDefault(r => r.UserId == currentUser.guid && !r.IsDeleted);
            if (recruiter == null)
                throw new UnauthorizedAccessException("Recruiter not found for current user.");

            if (post.RecruiterID != recruiter.guid)
                throw new UnauthorizedAccessException("You are not authorized to update this job post.");

            var existingRequirements = post.requirements?.ToList() ?? new();
            foreach (var old in existingRequirements)
                await _requirementRepository.Delete(old.guid);

            var newEntities = _requirementMapper.MapList(updatedRequirements, postId);
            foreach (var requirement in newEntities)
                await _requirementRepository.Add(requirement);

            _dataLogger.Info($"Updated requirements for postId: {postId}");
        }
        catch (Exception ex)
        {
            _errorLogger.Error($"Error updating requirements for job post: {postId}", ex);
            throw new UpdateException($"Failed to update requirements: {ex.Message}");
        }
    }

    public async Task UpdatePostSkills(Guid postId, List<SkillRegisterDto> updatedSkills, ClaimsPrincipal user)
    {
        try
        {
            var post = await _jobPostRepository.Get(postId) ?? throw new RecordNotFoundException("Job post not found");

            // Authorization logic
            var username = user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("User ID not found in token.");

            var users = await _userRepository.GetAll();
            var currentUser = users.FirstOrDefault(u => u.Username == username);
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not found.");

            var recruiters = await _recruiterRepository.GetAll();
            var recruiter = recruiters.FirstOrDefault(r => r.UserId == currentUser.guid && !r.IsDeleted);
            if (recruiter == null)
                throw new UnauthorizedAccessException("Recruiter not found for current user.");

            if (post.RecruiterID != recruiter.guid)
                throw new UnauthorizedAccessException("You are not authorized to update this job post.");

            var allSkills = await _skillRepository.GetAll();
            var missingSkills = updatedSkills
                .Where(us => !allSkills.Any(s => s.Name.Equals(us.Name, StringComparison.OrdinalIgnoreCase)))
                .Select(us => us.Name)
                .ToList();

            if (missingSkills.Any())
                throw new UpdateException($"The following skills are invalid or not registered: {string.Join(", ", missingSkills)}");

            var existingSkills = post.requiredSkills?.ToList() ?? new();
            foreach (var ps in existingSkills)
                await _postSkillRepository.Delete(ps.guid);

            var matchedSkills = allSkills
                .Where(s => updatedSkills.Any(us => us.Name.Equals(s.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var newPostSkills = _postSkillMapper.MapSeekerSkills(postId, matchedSkills);
            foreach (var ps in newPostSkills)
                await _postSkillRepository.Add(ps);

            _dataLogger.Info($"Updated post skills for job post: {postId}");
        }
        catch (Exception ex)
        {
            _errorLogger.Error($"Failed to update post skills for job post: {postId}", ex);
            throw new UpdateException($"Failed to update post skills: {ex.Message}");
        }
    }
    public async Task<IEnumerable<JobPostDto>> FilterJobPostsByTitle(string title)
    {
        var allPosts = await _jobPostRepository.GetAll();
        var filtered = allPosts
            .Where(p => !p.IsDeleted && p.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
            .Select(p => jobPostResMapper.MapToDto(p))
            .ToList();
        return filtered;
    }
    public async Task<IEnumerable<JobPostDto>> FilterJobPostsByLocation(string location)
    {
        var allPosts = await _jobPostRepository.GetAll();
        var filtered = allPosts
            .Where(p => !p.IsDeleted && p.Location != null && p.Location.Contains(location, StringComparison.OrdinalIgnoreCase))
            .Select(p => jobPostResMapper.MapToDto(p))
            .ToList();
        return filtered;
    }


    public async Task<IEnumerable<JobPostDto>> FilterJobPostsBySalary(decimal? minSalary = null, decimal? maxSalary = null)
    {
        var allPosts = await _jobPostRepository.GetAll();

        var filtered = allPosts
            .Where(p => !p.IsDeleted && IsSalaryInRange(p.SalaryPackage, minSalary, maxSalary))
            .Select(p => jobPostResMapper.MapToDto(p))
            .ToList();

        return filtered;
    }


    private bool IsSalaryInRange(string? salaryPackage, decimal? minSalary, decimal? maxSalary)
    {
        if (string.IsNullOrWhiteSpace(salaryPackage))
            return false;

        var cleaned = salaryPackage.Replace("₹", "")
                                   .Replace("LPA", "")
                                   .Replace(",", "")
                                   .Trim();

        var rangeMatch = System.Text.RegularExpressions.Regex.Match(cleaned, @"(\d+(\.\d+)?)\s*-\s*(\d+(\.\d+)?)");
        if (rangeMatch.Success)
        {
            if (decimal.TryParse(rangeMatch.Groups[1].Value, out var postMin) &&
                decimal.TryParse(rangeMatch.Groups[3].Value, out var postMax))
            {
                return
                    (!minSalary.HasValue || postMax >= minSalary.Value) &&
                    (!maxSalary.HasValue || postMin <= maxSalary.Value);
            }
        }
        else
        {
            var singleMatch = System.Text.RegularExpressions.Regex.Match(cleaned, @"^\d+(\.\d+)?$");
            if (singleMatch.Success &&
                decimal.TryParse(singleMatch.Value, out var postSalary))
            {
                return
                    (!minSalary.HasValue || postSalary >= minSalary.Value) &&
                    (!maxSalary.HasValue || postSalary <= maxSalary.Value);
            }
        }

        return false;
    }


}
