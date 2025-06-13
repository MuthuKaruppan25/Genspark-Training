using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using Microsoft.AspNetCore.SignalR;

public class JobPostServiceTests
{
    private readonly Mock<IRepository<Guid, JobPost>> _jobPostRepoMock = new();
    private readonly Mock<IRepository<Guid, Recruiter>> _recruiterRepoMock = new();
    private readonly Mock<IRepository<Guid, Skill>> _skillRepoMock = new();
    private readonly Mock<IRepository<Guid, Requirements>> _requirementRepoMock = new();
    private readonly Mock<IRepository<Guid, Responsibilities>> _responsibilityRepoMock = new();
    private readonly Mock<IRepository<Guid, PostSkills>> _postSkillRepoMock = new();
    private readonly Mock<IJobPostPagedGet> _jobPostPagedGetMock = new();
    private readonly Mock<IRepository<Guid, User>> _userRepoMock = new();
    private readonly Mock<IRepository<Guid, Seeker>> _seekerRepoMock = new();
    private readonly Mock<ITransactionalJobPostService> _transactionalJobPostServiceMock = new();
    private readonly Mock<IHubContext<NotificationHub>> _hubContextMock = new();
    private readonly Mock<IClientProxy> _clientProxyMock = new();

    private readonly JobPostService _service;
    private readonly Mock<ISeekerNotificationService> _seekerNotificationServiceMock = new();

    public JobPostServiceTests()
    {
        var clientsMock = new Mock<IHubClients>();
        clientsMock.Setup(x => x.All).Returns(_clientProxyMock.Object);
        _hubContextMock.Setup(x => x.Clients).Returns(clientsMock.Object);

        _service = new JobPostService(
            _jobPostRepoMock.Object,
            _recruiterRepoMock.Object,
            _skillRepoMock.Object,
            _requirementRepoMock.Object,
            _responsibilityRepoMock.Object,
            _userRepoMock.Object,
            _seekerRepoMock.Object,
            _hubContextMock.Object,
            _transactionalJobPostServiceMock.Object,
            _seekerNotificationServiceMock.Object, // <-- add this line
            _jobPostPagedGetMock.Object,
            _postSkillRepoMock.Object
        );
    }

    [Fact]
    public async Task AddJobPost_Success()
    {
        var jobPostDto = new JobPostDto
        {
            Title = "Backend Developer",
            Description = "Develop APIs",
            EmploymentType = "Full-Time",
            EmploymentPosition = "Senior",
            Location = "Remote",
            SalaryPackage = "100k-120k",
            RecruiterId = Guid.NewGuid(),
            LastDate = DateTime.UtcNow.AddDays(30),
            requirements = new List<RequirementsAddDto> { new RequirementsAddDto { Name = "5+ years experience" } },
            responsibilities = new List<ResponsibilitiesAddDto> { new ResponsibilitiesAddDto { Name = "Write clean code" } },
            skills = new List<SkillRegisterDto> { new SkillRegisterDto { Name = "C#" } }
        };
        var response = new JobPostRegisterResponseDto
        {
            PostId = Guid.NewGuid(),
            Title = jobPostDto.Title,
            RecruiterId = jobPostDto.RecruiterId,
            PostedDate = DateTime.UtcNow,
            LastDate = jobPostDto.LastDate
        };

        _transactionalJobPostServiceMock.Setup(x => x.AddJobPostAsync(jobPostDto)).ReturnsAsync(response);
        _clientProxyMock.Setup(x => x.SendCoreAsync("ReceiveMessage", It.IsAny<object[]>(), default)).Returns(Task.CompletedTask);

        var result = await _service.AddJobPost(jobPostDto);

        Assert.Equal(jobPostDto.Title, result.Title);
        Assert.Equal(jobPostDto.RecruiterId, result.RecruiterId);
        Assert.Equal(jobPostDto.LastDate, result.LastDate);
    }

    [Fact]
    public async Task AddJobPost_ThrowsFieldRequiredException()
    {
        var jobPostDto = new JobPostDto();
        _transactionalJobPostServiceMock.Setup(x => x.AddJobPostAsync(jobPostDto)).ThrowsAsync(new FieldRequiredException("Field required"));

        await Assert.ThrowsAsync<FieldRequiredException>(() => _service.AddJobPost(jobPostDto));
    }
    [Fact]
    public async Task AddJobPost_ThrowsDuplicateEntryException()
    {
        var jobPostDto = new JobPostDto();
        _transactionalJobPostServiceMock
            .Setup(x => x.AddJobPostAsync(jobPostDto))
            .ThrowsAsync(new DuplicateEntryException("Duplicate entry"));

        await Assert.ThrowsAsync<DuplicateEntryException>(() => _service.AddJobPost(jobPostDto));
    }

    [Fact]
    public async Task AddJobPost_ThrowsRegistrationException_OnGeneralException()
    {
        var jobPostDto = new JobPostDto();
        _transactionalJobPostServiceMock
            .Setup(x => x.AddJobPostAsync(jobPostDto))
            .ThrowsAsync(new Exception("Unexpected error"));

        await Assert.ThrowsAsync<RegistrationException>(() => _service.AddJobPost(jobPostDto));
    }

    [Fact]
    public async Task GetPagedJobPosts_Success()
    {
        var pageData = new PageDataDto { pageNumber = 1, pageSize = 2 };
        var jobPosts = new List<JobPost>
        {
            new JobPost { guid = Guid.NewGuid(), Title = "Backend", Description = "API", IsDeleted = false },
            new JobPost { guid = Guid.NewGuid(), Title = "Frontend", Description = "UI", IsDeleted = false }
        };
        _jobPostPagedGetMock.Setup(x => x.GetPaged(pageData.pageNumber, pageData.pageSize)).ReturnsAsync(jobPosts);

        var result = await _service.GetPagedJobPosts(pageData);

        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedJobPosts_ThrowsFetchDataException_OnError()
    {
        var pageData = new PageDataDto { pageNumber = 1, pageSize = 2 };
        _jobPostPagedGetMock
            .Setup(x => x.GetPaged(pageData.pageNumber, pageData.pageSize))
            .ThrowsAsync(new Exception("db error"));

        await Assert.ThrowsAsync<FetchDataException>(() => _service.GetPagedJobPosts(pageData));
    }


    [Fact]
    public async Task GetJobPostsMatchingProfile_ThrowsFetchDataException_OnError()
    {
        var guid = Guid.NewGuid();
        _userRepoMock.Setup(x => x.GetAll()).ThrowsAsync(new Exception("db error"));

        await Assert.ThrowsAsync<FetchDataException>(() => _service.GetJobPostsMatchingProfile(guid, 1, 10));
    }
    [Fact]
    public async Task GetJobPostsByCompanyNameAsync_Success()
    {
        var company = new Company { CompanyName = "TestCo" };
        var recruiter = new Recruiter { company = company };
        var jobPost = new JobPost
        {
            guid = Guid.NewGuid(),
            Title = "Backend",
            recruiter = recruiter,
            IsDeleted = false
        };
        _jobPostRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<JobPost> { jobPost });

        var result = await _service.GetJobPostsByCompanyNameAsync("TestCo");

        Assert.Single(result);
        Assert.Equal("Backend", result.First().Title);
    }

    [Fact]
    public async Task GetJobPostsByCompanyNameAsync_ThrowsFetchDataException_OnError()
    {
        _jobPostRepoMock.Setup(x => x.GetAll()).ThrowsAsync(new Exception("Company not found"));

        await Assert.ThrowsAsync<FetchDataException>(() => _service.GetJobPostsByCompanyNameAsync("NonExistentCo"));
    }

    [Fact]
    public async Task GetJobPostByIdAsync_Success()
    {
        var postId = Guid.NewGuid();
        var jobPost = new JobPost
        {
            guid = postId,
            Title = "Backend",
            Description = "API"
        };
        _jobPostRepoMock.Setup(x => x.Get(postId)).ReturnsAsync(jobPost);

        var result = await _service.GetJobPostByIdAsync(postId);

        Assert.Equal("Backend", result.Title);
        Assert.Equal("API", result.Description);
    }

    [Fact]
    public async Task GetJobPostByIdAsync_ThrowsRecordNotFoundException()
    {
        var postId = Guid.NewGuid();
        _jobPostRepoMock
            .Setup(x => x.Get(postId))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.GetJobPostByIdAsync(postId));
    }

    
    [Fact]
    public async Task SoftDeleteJobPost_Success()
    {
        var postId = Guid.NewGuid();
        var recruiterId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { guid = userId, Username = "rec@mail.com" };
        var recruiter = new Recruiter { guid = recruiterId, UserId = userId, IsDeleted = false };
        var jobPost = new JobPost { guid = postId, RecruiterID = recruiterId, IsDeleted = false };

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "rec@mail.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _jobPostRepoMock.Setup(x => x.Get(postId)).ReturnsAsync(jobPost);
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _recruiterRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Recruiter> { recruiter });
        _jobPostRepoMock.Setup(x => x.Update(postId, It.IsAny<JobPost>())).ReturnsAsync(jobPost);

        var result = await _service.SoftDeleteJobPost(postId, principal);

        Assert.True(result);
        _jobPostRepoMock.Verify(x => x.Update(postId, It.Is<JobPost>(jp => jp.IsDeleted)), Times.Once);
    }

    [Fact]
    public async Task SoftDeleteJobPost_ThrowsRecordNotFoundException_WhenJobPostNotFound()
    {
        var postId = Guid.NewGuid();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "rec@mail.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _jobPostRepoMock.Setup(x => x.Get(postId)).ReturnsAsync((JobPost)null);

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.SoftDeleteJobPost(postId, principal));
    }

    [Fact]
    public async Task SoftDeleteJobPost_ThrowsUnauthorizedAccessException_WhenNotAuthorized()
    {
        var postId = Guid.NewGuid();
        var recruiterId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { guid = userId, Username = "rec@mail.com" };
        var recruiter = new Recruiter { guid = recruiterId, UserId = userId, IsDeleted = false };
        var jobPost = new JobPost { guid = postId, RecruiterID = Guid.NewGuid(), IsDeleted = false }; // RecruiterID does not match

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "rec@mail.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _jobPostRepoMock.Setup(x => x.Get(postId)).ReturnsAsync(jobPost);
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _recruiterRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Recruiter> { recruiter });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.SoftDeleteJobPost(postId, principal));
    }

    [Fact]
    public async Task UpdateJobPost_Success()
    {
        var postId = Guid.NewGuid();
        var recruiterId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var jobPost = new JobPost
        {
            guid = postId,
            Title = "Old",
            Description = "Old",
            Location = "Old",
            SalaryPackage = "Old",
            EmploymentType = "Old",
            EmploymentPosition = "Old",
            LastDate = DateTime.UtcNow.AddDays(10),
            PostedDate = DateTime.UtcNow.AddDays(-10),
            IsDeleted = false,
            RecruiterID = recruiterId
        };
        var updateDto = new JobPostUpdateDto
        {
            Title = "New",
            Description = "New Desc",
            Location = "New Loc",
            SalaryPackage = "120k",
            EmploymentType = "Full-Time",
            EmploymentPosition = "Lead",
            ClosingDate = DateTime.UtcNow.AddDays(20)
        };
        var user = new User { guid = userId, Username = "rec@mail.com" };
        var recruiter = new Recruiter { guid = recruiterId, UserId = userId, IsDeleted = false };
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "rec@mail.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _jobPostRepoMock.Setup(x => x.Get(postId)).ReturnsAsync(jobPost);
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _recruiterRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Recruiter> { recruiter });
        _jobPostRepoMock.Setup(x => x.Update(postId, It.IsAny<JobPost>())).ReturnsAsync(jobPost);

        // Call the method
        var result = await _service.UpdateJobPost(postId, updateDto, principal);

        Assert.Equal(updateDto.Title, result.Title);
        Assert.Equal(updateDto.Description, result.Description);
        Assert.Equal(updateDto.Location, result.Location);
        Assert.Equal(updateDto.SalaryPackage, result.SalaryPackage);
        Assert.Equal(updateDto.EmploymentType, result.EmploymentType);
        Assert.Equal(updateDto.EmploymentPosition, result.EmploymentPosition);
        Assert.Equal(updateDto.ClosingDate, result.LastDate);
    }
}