using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using System.Security.Claims;

public class RecruiterServiceTests
{
    private readonly Mock<IRepository<Guid, Recruiter>> _recruiterRepoMock = new();
    private readonly Mock<IRepository<Guid, User>> _userRepoMock = new();
    private readonly Mock<IRepository<Guid, Company>> _companyRepoMock = new();
    private readonly Mock<IRepository<Guid, JobPost>> _jobPostRepoMock = new();
    private readonly Mock<IEncryptionService> _encryptionMock = new();
    private readonly Mock<ITransactionalRecruiterRegister> _transactionalRecruiterRegisterMock = new();

    private readonly RecruiterService _service;

    public RecruiterServiceTests()
    {
        _service = new RecruiterService(
            _recruiterRepoMock.Object,
            _userRepoMock.Object,
            _companyRepoMock.Object,
            _transactionalRecruiterRegisterMock.Object,
            _jobPostRepoMock.Object,
            _encryptionMock.Object
        );
    }

    [Fact]
    public async Task RegisterCompany_Success()
    {
        var dto = new RecruiterRegisterDto
        {
            Email = "rec@company.com",
            Password = "pw",
            FirstName = "Rec",
            LastName = "Ruiter",
            PhoneNumber = "1234567890",
            Designation = "HR",
            CompanyName = "TestCo"
        };
        var response = new RecruiterRegisterResponseDto
        {
            FirstName = "Rec",
            LastName = "Ruiter",
            CompanyName = "TestCo"
        };

        _transactionalRecruiterRegisterMock
            .Setup(x => x.RegisterCompany(dto))
            .ReturnsAsync(response);

        var result = await _service.RegisterCompany(dto);

        Assert.Equal("Rec", result.FirstName);
        Assert.Equal("Ruiter", result.LastName);
        Assert.Equal("TestCo", result.CompanyName);
    }

    [Fact]
    public async Task RegisterCompany_ThrowsDuplicateEntryException()
    {
        var dto = new RecruiterRegisterDto();
        _transactionalRecruiterRegisterMock
            .Setup(x => x.RegisterCompany(dto))
            .ThrowsAsync(new DuplicateEntryException("dup"));

        await Assert.ThrowsAsync<DuplicateEntryException>(() => _service.RegisterCompany(dto));
    }

    [Fact]
    public async Task RegisterCompany_ThrowsFieldRequiredException()
    {
        var dto = new RecruiterRegisterDto();
        _transactionalRecruiterRegisterMock
            .Setup(x => x.RegisterCompany(dto))
            .ThrowsAsync(new FieldRequiredException("field required"));

        await Assert.ThrowsAsync<FieldRequiredException>(() => _service.RegisterCompany(dto));
    }

    [Fact]
    public async Task RegisterCompany_ThrowsGeneralRegistrationException()
    {
        var dto = new RecruiterRegisterDto();
        _transactionalRecruiterRegisterMock
            .Setup(x => x.RegisterCompany(dto))
            .ThrowsAsync(new RegistrationException("general error"));

        await Assert.ThrowsAsync<RegistrationException>(() => _service.RegisterCompany(dto));
    }

    [Fact]
    public async Task GetRecruiterById_Success()
    {
        var recruiterId = Guid.NewGuid();
        var recruiter = new Recruiter
        {
            guid = recruiterId,
            FirstName = "Rec",
            LastName = "Ruiter",
            PhoneNumber = "1234567890",
            Designation = "HR",
            CompanyId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            IsDeleted = false
        };
        _recruiterRepoMock.Setup(x => x.Get(recruiterId)).ReturnsAsync(recruiter);

        var result = await _service.GetRecruiterById(recruiterId);

        Assert.Equal(recruiterId, result.guid);
        Assert.Equal("Rec", result.FirstName);
        Assert.False(result.IsDeleted);
    }

    [Fact]
    public async Task GetRecruiterById_ThrowsRecordNotFoundException()
    {
        var recruiterId = Guid.NewGuid();
        _recruiterRepoMock.Setup(x => x.Get(recruiterId)).ReturnsAsync((Recruiter)null);

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.GetRecruiterById(recruiterId));
    }

    [Fact]
    public async Task GetRecruiterJobPosts_Success()
    {
        var recruiterId = Guid.NewGuid();
        var jobPost = new JobPost
        {
            guid = Guid.NewGuid(),
            Title = "Dev",
            Description = "Dev job",
            RecruiterID = recruiterId,
            IsDeleted = false
        };
        _jobPostRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<JobPost> { jobPost });

        var result = await _service.GetRecruiterJobPosts(recruiterId);

        Assert.Single(result);
        Assert.Equal("Dev", result.First().Title);
    }

    [Fact]
    public async Task GetRecruiterByUsername_Success()
    {
        var userId = Guid.NewGuid();
        var user = new User { guid = userId, Username = "rec@company.com" };
        var recruiter = new Recruiter
        {
            guid = Guid.NewGuid(),
            UserId = userId,
            FirstName = "Rec",
            LastName = "Ruiter",
            IsDeleted = false
        };
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _recruiterRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Recruiter> { recruiter });

        var result = await _service.GetRecruiterByUsername("rec@company.com");

        Assert.Equal(recruiter.guid, result.guid);
        Assert.Equal("Rec", result.FirstName);
    }

    [Fact]
    public async Task GetRecruiterByUsername_UserNotFound()
    {
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.GetRecruiterByUsername("rec@company.com"));
    }

    [Fact]
    public async Task GetRecruiterByUsername_RecruiterNotFound()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "rec@company.com" };
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _recruiterRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Recruiter>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.GetRecruiterByUsername("rec@company.com"));
    }

    [Fact]
    public async Task UpdateRecruiterDetails_Success()
    {
        var recruiterId = Guid.NewGuid();
        var recruiter = new Recruiter
        {
            guid = recruiterId,
            FirstName = "Old",
            LastName = "Name",
            PhoneNumber = "111",
            Designation = "Old",
            CompanyId = Guid.NewGuid(),
            IsDeleted = false
        };
        var updateDto = new RecruiterUpdateDto
        {
            FirstName = "New",
            LastName = "Name",
            PhoneNumber = "222",
            Designation = "New",
            CompanyId = Guid.NewGuid()
        };
        _recruiterRepoMock.Setup(x => x.Get(recruiterId)).ReturnsAsync(recruiter);
        _recruiterRepoMock.Setup(x => x.Update(recruiterId, It.IsAny<Recruiter>())).ReturnsAsync(recruiter);

        await _service.UpdateRecruiterDetails(recruiterId, updateDto);

        _recruiterRepoMock.Verify(x => x.Update(recruiterId, It.Is<Recruiter>(r =>
            r.FirstName == updateDto.FirstName &&
            r.LastName == updateDto.LastName &&
            r.PhoneNumber == updateDto.PhoneNumber &&
            r.Designation == updateDto.Designation &&
            r.CompanyId == updateDto.CompanyId
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateRecruiterDetails_ThrowsRecordNotFoundException()
    {
        var recruiterId = Guid.NewGuid();
        _recruiterRepoMock.Setup(x => x.Get(recruiterId)).ReturnsAsync((Recruiter)null);

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.UpdateRecruiterDetails(recruiterId, new RecruiterUpdateDto()));
    }

    [Fact]
    public async Task SoftDeleteRecruiter_Success()
    {
        var recruiterId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var recruiter = new Recruiter { guid = recruiterId, IsDeleted = false, UserId = userId };
        var user = new User { guid = userId, Username = "rec@company.com" };

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "rec@company.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _recruiterRepoMock.Setup(x => x.Get(recruiterId)).ReturnsAsync(recruiter);
        _recruiterRepoMock.Setup(x => x.Update(recruiterId, It.IsAny<Recruiter>())).ReturnsAsync(recruiter);

        await _service.SoftDeleteRecruiter(recruiterId, principal);

        _recruiterRepoMock.Verify(x => x.Update(recruiterId, It.Is<Recruiter>(r => r.IsDeleted)), Times.Once);
    }

    [Fact]
    public async Task SoftDeleteRecruiter_ThrowsRecordNotFoundException()
    {
        var recruiterId = Guid.NewGuid();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "rec@company.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _recruiterRepoMock.Setup(x => x.Get(recruiterId)).ReturnsAsync((Recruiter)null);
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.SoftDeleteRecruiter(recruiterId, principal));
    }

    [Fact]
    public async Task SoftDeleteRecruiter_ThrowsUnauthorizedAccessException()
    {
        var recruiterId = Guid.NewGuid();
        var recruiter = new Recruiter { guid = recruiterId, IsDeleted = false, UserId = Guid.NewGuid() };
        var user = new User { guid = Guid.NewGuid(), Username = "someoneelse@company.com" };

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "notowner@company.com") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _recruiterRepoMock.Setup(x => x.Get(recruiterId)).ReturnsAsync(recruiter);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.SoftDeleteRecruiter(recruiterId, principal));
    }
}