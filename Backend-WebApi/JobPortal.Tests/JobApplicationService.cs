using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using Microsoft.AspNetCore.Http;

public class JobApplicantServiceTests
{
    private readonly Mock<IRepository<Guid, JobApplication>> _jobAppRepoMock = new();
    private readonly Mock<IRepository<Guid, User>> _userRepoMock = new();
    private readonly Mock<IRepository<Guid, Seeker>> _seekerRepoMock = new();
    private readonly Mock<IJobApplicationPaged> _jobAppPagedMock = new();
    private readonly Mock<IFileService> _fileServiceMock = new();
    private readonly Mock<IRepository<Guid, JobPost>> _jobPostRepoMock = new();

    private readonly JobApplicantService _service;

    public JobApplicantServiceTests()
    {
        _service = new JobApplicantService(
            _jobAppRepoMock.Object,
            _userRepoMock.Object,
            _jobAppPagedMock.Object,
            _jobPostRepoMock.Object,
            _fileServiceMock.Object,
            _seekerRepoMock.Object
            
        );
    }
    [Fact]
    public async Task CreateApplication_Success()
    {
        var userId = Guid.NewGuid();
        var seekerId = Guid.NewGuid();
        var jobPostId = Guid.NewGuid();

        var user = new User { guid = userId, Username = "seeker@mail.com" };
        var seeker = new Seeker { guid = seekerId, UserId = userId };
        var resumeMock = new Mock<IFormFile>();
        var dto = new JobApplicantAddDto
        {
            JobPostId = jobPostId,
            username = "seeker@mail.com",
            Resume = resumeMock.Object
        };
        var jobApp = new JobApplication
        {
            guid = Guid.NewGuid(),
            JobPostId = jobPostId,
            SeekerId = seekerId,
            AppliedOn = DateTime.UtcNow,
            Status = "Applied"
        };
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var jobPost = new JobPost { guid = dto.JobPostId, IsDeleted = false, LastDate = DateTime.UtcNow.AddDays(5) };
        _jobPostRepoMock.Setup(x => x.Get(dto.JobPostId)).ReturnsAsync(jobPost);

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker> { seeker });
        _jobAppRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<JobApplication>());
        _jobAppRepoMock.Setup(x => x.Add(It.IsAny<JobApplication>())).ReturnsAsync(jobApp);
        _fileServiceMock.Setup(x => x.UploadFileAsync(It.IsAny<FileUploadDto>()))
            .ReturnsAsync(new FileUploadResponseDto { FileName = "resume.pdf" });

        var result = await _service.CreateApplication(dto, principal);

        Assert.Equal(jobApp.JobPostId, result.JobPostId);
        Assert.Equal(jobApp.SeekerId, result.SeekerId);
        Assert.Equal("Applied", result.Status);
        Assert.True((result.AppliedOn - jobApp.AppliedOn).TotalSeconds < 5);
    }
    [Fact]
    public async Task CreateApplication_ThrowsRegistrationException_WhenUsernameMismatch()
    {
        var dto = new JobApplicantAddDto { username = "otheruser@mail.com", JobPostId = Guid.NewGuid(), Resume = new Mock<IFormFile>().Object };
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        await Assert.ThrowsAsync<RegistrationException>(() => _service.CreateApplication(dto, principal));
    }

    [Fact]
    public async Task CreateApplication_ThrowsRecordNotFoundException_WhenUserNotFound()
    {
        var dto = new JobApplicantAddDto { username = "nouser", JobPostId = Guid.NewGuid(), Resume = new Mock<IFormFile>().Object };
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "nouser") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.CreateApplication(dto, principal));
    }

    [Fact]
    public async Task CreateApplication_ThrowsRecordNotFoundException_WhenSeekerNotFound()
    {
        var userId = Guid.NewGuid();
        var user = new User { guid = userId, Username = "seeker@mail.com" };
        var dto = new JobApplicantAddDto { username = "seeker@mail.com", JobPostId = Guid.NewGuid(), Resume = new Mock<IFormFile>().Object };
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.CreateApplication(dto, principal));
    }

    [Fact]
    public async Task CreateApplication_ThrowsDuplicateEntryException_WhenDuplicateExists()
    {
        var userId = Guid.NewGuid();
        var seekerId = Guid.NewGuid();
        var jobPostId = Guid.NewGuid();
        var user = new User { guid = userId, Username = "seeker@mail.com" };
        var seeker = new Seeker { guid = seekerId, UserId = userId };
        var dto = new JobApplicantAddDto { username = "seeker@mail.com", JobPostId = jobPostId, Resume = new Mock<IFormFile>().Object };
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var existingApp = new JobApplication { SeekerId = seekerId, JobPostId = jobPostId, IsDeleted = false };

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker> { seeker });
        _jobAppRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<JobApplication> { existingApp });

        await Assert.ThrowsAsync<DuplicateEntryException>(() => _service.CreateApplication(dto, principal));
    }

    

    [Fact]
    public async Task GetPagedApplications_Success()
    {
        var jobApp = new JobApplication
        {
            guid = Guid.NewGuid(),
            JobPostId = Guid.NewGuid(),
            SeekerId = Guid.NewGuid(),
            AppliedOn = DateTime.UtcNow,
            Status = "Applied"
        };
        _jobAppPagedMock.Setup(x => x.GetPaged(1, 10)).ReturnsAsync(new List<JobApplication> { jobApp });

        var result = await _service.GetPagedApplications(1, 10);

        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedApplications_ThrowsFetchDataException_OnError()
    {
        _jobAppPagedMock.Setup(x => x.GetPaged(1, 10)).ThrowsAsync(new Exception("db error"));

        await Assert.ThrowsAsync<FetchDataException>(() => _service.GetPagedApplications(1, 10));
    }

    [Fact]
    public async Task SoftDeleteApplication_Success()
    {
        var appId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var seekerId = Guid.NewGuid();
        var user = new User { guid = userId, Username = "seeker@mail.com" };
        var seeker = new Seeker { guid = seekerId, UserId = userId };
        var jobApp = new JobApplication { guid = appId, SeekerId = seekerId, IsDeleted = false };

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _jobAppRepoMock.Setup(x => x.Get(appId)).ReturnsAsync(jobApp);
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker> { seeker });
        _jobAppRepoMock.Setup(x => x.Update(appId, It.IsAny<JobApplication>())).ReturnsAsync(jobApp);

        var result = await _service.SoftDeleteApplication(appId, principal);

        Assert.True(result);
        _jobAppRepoMock.Verify(x => x.Update(appId, It.Is<JobApplication>(a => a.IsDeleted)), Times.Once);
    }

    [Fact]
    public async Task SoftDeleteApplication_ThrowsRecordNotFoundException_WhenNotFound()
    {
        var appId = Guid.NewGuid();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        _jobAppRepoMock.Setup(x => x.Get(appId)).ReturnsAsync((JobApplication)null);

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.SoftDeleteApplication(appId, principal));
    }

    [Fact]
    public async Task SoftDeleteApplication_ThrowsUnauthorizedAccessException_WhenNotAuthorized()
    {
        var appId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var seekerId = Guid.NewGuid();
        var user = new User { guid = userId, Username = "seeker@mail.com" };
        var seeker = new Seeker { guid = seekerId, UserId = userId };
        var jobApp = new JobApplication { guid = appId, SeekerId = Guid.NewGuid(), IsDeleted = false }; // SeekerId does not match

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _jobAppRepoMock.Setup(x => x.Get(appId)).ReturnsAsync(jobApp);
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker> { seeker });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.SoftDeleteApplication(appId, principal));
    }

    [Fact]
    public async Task SoftDeleteApplication_ThrowsUpdateException_OnGeneralError()
    {
        var appId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var seekerId = Guid.NewGuid();
        var user = new User { guid = userId, Username = "seeker@mail.com" };
        var seeker = new Seeker { guid = seekerId, UserId = userId };
        var jobApp = new JobApplication { guid = appId, SeekerId = seekerId, IsDeleted = false };

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _jobAppRepoMock.Setup(x => x.Get(appId)).ReturnsAsync(jobApp);
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker> { seeker });
        _jobAppRepoMock.Setup(x => x.Update(appId, It.IsAny<JobApplication>())).ThrowsAsync(new Exception("db error"));

        await Assert.ThrowsAsync<UpdateException>(() => _service.SoftDeleteApplication(appId, principal));
    }
}