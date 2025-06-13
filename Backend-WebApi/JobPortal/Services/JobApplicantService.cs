using System.Security.Claims;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using log4net;

public class JobApplicantService : IJobApplicantService
{
    private readonly IRepository<Guid, JobApplication> _jobApplicationRepository;
    private readonly IRepository<Guid, User> _userRepository;
    private readonly IRepository<Guid, Seeker> _seekerRepository;
    private readonly IRepository<Guid, JobPost> _jobPostRepository;
    private readonly IJobApplicationPaged _jobApplicationPaged;
    private readonly IFileService _fileService;
    private readonly JobApplicationMapper _jobApplicationMapper;
    private readonly JobApplicationDetailsMapper jobApplicationDetailsMapper;

    private static readonly ILog _errorLogger = LogManager.GetLogger("ErrorFileAppender");
    private static readonly ILog _dataLogger = LogManager.GetLogger("DataChangeFileAppender");

    public JobApplicantService(
        IRepository<Guid, JobApplication> jobApplicationRepository,
        IRepository<Guid, User> userRepository,
        IJobApplicationPaged jobApplicationPaged,
        IRepository<Guid,JobPost> jobPostRepository,
        IFileService fileService,
        IRepository<Guid, Seeker> seekerRepository)
    {
        _jobApplicationRepository = jobApplicationRepository;
        _userRepository = userRepository;
        _seekerRepository = seekerRepository;
        _jobPostRepository = jobPostRepository;
        _jobApplicationPaged = jobApplicationPaged;
        _fileService = fileService;
        jobApplicationDetailsMapper = new JobApplicationDetailsMapper();
        _jobApplicationMapper = new JobApplicationMapper();
    }

    public async Task<JobApplicantAddResponse> CreateApplication(JobApplicantAddDto jobApplicantAddDto,ClaimsPrincipal userPrincipal)
    {
        try
        {
            var claimUsername = userPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claimUsername == null || !claimUsername.Equals(jobApplicantAddDto.username, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("You are not authorized to delete this seeker.");
            var users = await _userRepository.GetAll();
            var user = users.FirstOrDefault(u => u.Username.Equals(jobApplicantAddDto.username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
                throw new RecordNotFoundException("User not found");

            var seekers = await _seekerRepository.GetAll();
            var seeker = seekers.FirstOrDefault(s => s.UserId == user.guid);
            if (seeker == null)
                throw new RecordNotFoundException("Seeker not found");


            var existingApplications = await _jobApplicationRepository.GetAll();
            bool duplicateExists = existingApplications.Any(app =>
                app.SeekerId == seeker.guid &&
                app.JobPostId == jobApplicantAddDto.JobPostId &&
                !app.IsDeleted);

            if (duplicateExists)
                throw new DuplicateEntryException("You have already applied for this job.");
            var jobPost = await _jobPostRepository.Get(jobApplicantAddDto.JobPostId);
            if (jobPost == null || jobPost.IsDeleted)
                throw new RecordNotFoundException("Job post not found or is deleted.");
            if (jobPost.LastDate < DateTime.UtcNow)
                throw new ValidationException("You cannot apply for this job as the last date has passed.");

            var jobApplication = _jobApplicationMapper.Map(jobApplicantAddDto, seeker.guid);

            await _jobApplicationRepository.Add(jobApplication!);
            var file = await _fileService.UploadFileAsync(new FileUploadDto
            {
                JobPostId = jobApplicantAddDto.JobPostId,
                SeekerId = seeker.guid,
                Resume = jobApplicantAddDto.Resume!
            });

            _dataLogger.Info($"Job application created for SeekerId = {seeker.guid}, JobPostId = {jobApplicantAddDto.JobPostId}");

            return _jobApplicationMapper.MapToResponse(jobApplication!)!;
        }
        catch (ValidationException ex)
        {
            _errorLogger.Error("Validation error during job application creation", ex);
            throw;
        }
        catch (FieldRequiredException ex)
        {
            _errorLogger.Error("Field required exception during job application creation", ex);
            throw new ValidationException("Required field is missing", ex);
        }
        catch (DuplicateEntryException ex)
        {
            _errorLogger.Error("Duplicate job application attempt", ex);
            throw;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Record not found during job application creation", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Error creating job application", ex);
            throw new RegistrationException("Failed to create job application", ex);
        }
    }
    public async Task<PagedResult<JobApplicationDetailsDto>> GetPagedApplications(int pageNumber, int pageSize)
    {
        try
        {
            var applications = await _jobApplicationPaged.GetPaged(pageNumber, pageSize);
            var mapped = jobApplicationDetailsMapper.MapList(applications)
                .OrderByDescending(dto => dto.AppliedOn)
                .ToList();

            return new PagedResult<JobApplicationDetailsDto>
            {
                Items = mapped,
                TotalCount = mapped.Count
            };
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Error fetching paged job applications", ex);
            throw new FetchDataException($"Error fetching paged job applications: {ex.Message}");
        }
    }

    public async Task<bool> SoftDeleteApplication(Guid applicationId, ClaimsPrincipal user)
    {
        try
        {
            var application = await _jobApplicationRepository.Get(applicationId);
            if (application == null || application.IsDeleted)
            {
                throw new RecordNotFoundException("Job application not found");
            }


            var username = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }


            var users = await _userRepository.GetAll();
            var userDetails = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            var seekers = await _seekerRepository.GetAll();
            var seeker = seekers.FirstOrDefault(s => s.UserId == userDetails?.guid);
            if (seeker == null)
            {
                throw new UnauthorizedAccessException("Seeker not found for the current user.");
            }


            if (application.SeekerId != seeker.guid)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this application.");
            }

            application.IsDeleted = true;

            await _jobApplicationRepository.Update(applicationId, application);

            _dataLogger.Info($"Job application soft-deleted: ApplicationId = {applicationId}, SeekerId = {seeker.guid}");

            return true;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Job application not found for deletion", ex);
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            _errorLogger.Error("Unauthorized delete attempt", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to soft delete job application", ex);
            throw new UpdateException($"Failed to soft delete job application: {ex.Message}");
        }
    }

}
