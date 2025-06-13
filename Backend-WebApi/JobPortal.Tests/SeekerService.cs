using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using Moq;
using Xunit;

public class SeekerServiceTests
{
    private readonly Mock<IRepository<Guid, User>> _userRepoMock = new();
    private readonly Mock<IRepository<Guid, Skill>> _skillRepoMock = new();
    private readonly Mock<IRepository<Guid, Seeker>> _seekerRepoMock = new();
    private readonly Mock<IRepository<Guid, SeekerSkills>> _seekerSkillsRepoMock = new();
    private readonly Mock<IEncryptionService> _encryptionMock = new();
    private readonly Mock<ISeekerPagedGet> _seekerPagedGetMock = new();
    private readonly Mock<ITransactionalSeekerService> _transactionalSeekerServiceMock = new();

    private readonly SeekerService _service;

    public SeekerServiceTests()
    {
        _service = new SeekerService(
            _userRepoMock.Object,
            _skillRepoMock.Object,
            _seekerRepoMock.Object,
            _seekerSkillsRepoMock.Object,
            _transactionalSeekerServiceMock.Object,
            _seekerPagedGetMock.Object,
            _encryptionMock.Object
        );
    }

    [Fact]
    public async Task RegisterSeeker_Success()
    {
        var dto = new SeekerRegisterDto
        {
            Email = "a@b.com",
            Password = "pw",
            FirstName = "A",
            LastName = "B",
            Experience = 5,
            About = "About A",
            Education = "Bachelor's",
            skills = new List<SkillRegisterDto>
        {
            new() { Name = "C#" },
            new() { Name = "SQL" }
        }
        };

        var response = new SeekerRegisterResponseDto
        {
            FirstName = "A",
            LastName = "B",
            Experience = 5,
            Education = "Bachelor's",
            skills = new List<string> { "C#", "SQL" }
        };

        _transactionalSeekerServiceMock
            .Setup(x => x.RegisterSeekerWithTransaction(dto))
            .ReturnsAsync(response);

        var result = await _service.RegisterSeeker(dto);

        Assert.Equal("A", result.FirstName);
    }

    [Fact]
    public async Task RegisterSeeker_ThrowsDuplicateEntryException()
    {
        var dto = new SeekerRegisterDto();
        _transactionalSeekerServiceMock.Setup(x => x.RegisterSeekerWithTransaction(dto)).ThrowsAsync(new DuplicateEntryException("dup"));

        await Assert.ThrowsAsync<DuplicateEntryException>(() => _service.RegisterSeeker(dto));
    }

    [Fact]
    public async Task RegisterSeeker_ThrowsFieldRequiredException()
    {
        var dto = new SeekerRegisterDto();
        _transactionalSeekerServiceMock.Setup(x => x.RegisterSeekerWithTransaction(dto)).ThrowsAsync(new FieldRequiredException("field"));

        await Assert.ThrowsAsync<FieldRequiredException>(() => _service.RegisterSeeker(dto));
    }

    [Fact]
    public async Task RegisterSeeker_ThrowsGeneralException()
    {
        var dto = new SeekerRegisterDto();
        _transactionalSeekerServiceMock.Setup(x => x.RegisterSeekerWithTransaction(dto)).ThrowsAsync(new Exception("fail"));

        await Assert.ThrowsAsync<RegistrationException>(() => _service.RegisterSeeker(dto));
    }

    [Fact]
    public async Task GetPagedSeekers_Success()
    {
        var seekers = new List<Seeker> { new() { FirstName = "A", seekerSkills = new List<SeekerSkills> { new() { skill = new Skill { Name = "C#" } } } } };
        _seekerPagedGetMock.Setup(x => x.GetPaged(1, 1)).ReturnsAsync(seekers);

        var result = await _service.GetPagedSeekers(1, 1);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetPagedSeekers_ThrowsNoRecordsFoundException()
    {
        _seekerPagedGetMock.Setup(x => x.GetPaged(1, 1)).ReturnsAsync(new List<Seeker>());

        await Assert.ThrowsAsync<NoRecordsFoundException>(() => _service.GetPagedSeekers(1, 1));
    }

    [Fact]
    public async Task GetSeekerWithApplications_Success()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "abc", PasswordHash = new byte[] { 1 }, HashKey = new byte[] { 2 }, Role = "Seeker" };
        var jobPost = new JobPost { Title = "T", Description = "D" };
        var seeker = new Seeker
        {
            guid = Guid.NewGuid(),
            UserId = user.guid,
            FirstName = "A",
            LastName = "B",
            Experience = 5,
            Education = "Bachelor's",
            jobApplications = new List<JobApplication>()
        };
        var application = new JobApplication
        {
            guid = Guid.NewGuid(),
            JobPostId = jobPost.guid,
            SeekerId = user.guid,
            Status = "Applied",
            IsDeleted = false,
            AppliedOn = DateTime.UtcNow
        };
        seeker.jobApplications.Add(application);

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker> { seeker });

        var result = await _service.GetSeekerWithApplications("abc");

        Assert.Equal(seeker.guid, result.SeekerId);
        Assert.Single(result.Applications);
    }


    [Fact]
    public async Task GetSeekerWithApplications_SeekerNotFound()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "abc", PasswordHash = new byte[] { 1 }, HashKey = new byte[] { 2 }, Role = "Seeker" };
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.GetSeekerWithApplications("abc"));
    }

    [Fact]
    public async Task GetSeekerSkills_Success()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "abc", PasswordHash = new byte[] { 1 }, HashKey = new byte[] { 2 }, Role = "Seeker" };
        var skillId = Guid.NewGuid();
        var seeker = new Seeker
        {
            guid = Guid.NewGuid(),
            UserId = user.guid,
            FirstName = "A",
            LastName = "B",
            Experience = 5,
            Education = "Bachelor's",
            seekerSkills = new List<SeekerSkills> { new() { SkillId = skillId } }
        };
        var skill = new Skill { guid = skillId, Name = "C#" };
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker> { seeker });
        _skillRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Skill> { skill });

        var result = await _service.GetSeekerSkills("abc");

        Assert.Single(result);
        Assert.Equal("C#", result[0].Name);
    }

    [Fact]
    public async Task GetSeekerSkills_UserNotFound()
    {
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.GetSeekerSkills("abc"));
    }

    [Fact]
    public async Task GetSeekerSkills_SeekerNotFound()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "abc", PasswordHash = new byte[] { 1 }, HashKey = new byte[] { 2 }, Role = "Seeker" };
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.GetSeekerSkills("abc"));
    }

    [Fact]
    public async Task UpdateSeekerDetails_Success()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "abc", PasswordHash = new byte[] { 1 }, HashKey = new byte[] { 2 }, Role = "Seeker" };
        var seeker = new Seeker
        {
            guid = Guid.NewGuid(),
            UserId = user.guid,
            FirstName = "OldFirst",
            LastName = "OldLast",
            Experience = 2,
            Education = "Old Education",
            IsDeleted = false
        };
                var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "abc") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker> { seeker });
        _seekerRepoMock.Setup(x => x.Update(seeker.guid, It.IsAny<Seeker>())).ReturnsAsync(seeker);

        var dto = new SeekerUpdateDto
        {
            FirstName = "A",
            LastName = "B",
            Education = "E",
            Experience = 1,

        };

        await _service.UpdateSeekerDetails("abc", dto,principal);

        _seekerRepoMock.Verify(x => x.Update(seeker.guid, It.IsAny<Seeker>()), Times.Once);
    }

    [Fact]
    public async Task UpdateSeekerDetails_UserNotFound()
    {
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());
                var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "abc") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.UpdateSeekerDetails("abc", new SeekerUpdateDto(),principal));
    }

    [Fact]
    public async Task UpdateSeekerDetails_SeekerNotFound()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "abc", PasswordHash = new byte[] { 1 }, HashKey = new byte[] { 2 }, Role = "Seeker" };
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker>());
                var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "abc") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.UpdateSeekerDetails("abc", new SeekerUpdateDto(),principal));
    }

    [Fact]
    public async Task UpdateSeekerSkills_Success()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "abc", PasswordHash = new byte[] { 1 }, HashKey = new byte[] { 2 }, Role = "Seeker" };
        var seeker = new Seeker
        {
            guid = Guid.NewGuid(),
            UserId = user.guid,
            seekerSkills = new List<SeekerSkills> { new() { SkillId = Guid.NewGuid() } }
        };
        var skill = new Skill { guid = Guid.NewGuid(), Name = "C#" };
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "abc") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker> { seeker });
        _skillRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Skill> { skill });
        _seekerSkillsRepoMock.Setup(x => x.Delete(It.IsAny<Guid>())).ReturnsAsync(new SeekerSkills());
        _seekerSkillsRepoMock.Setup(x => x.Add(It.IsAny<SeekerSkills>())).ReturnsAsync(new SeekerSkills());

        var skills = new List<SkillRegisterDto>
    {
        new() { Name = "C#" }
    };

        await _service.UpdateSeekerSkills("abc", skills,principal);

        _seekerSkillsRepoMock.Verify(x => x.Add(It.Is<SeekerSkills>(ss =>
            ss.SkillId == skill.guid &&
            ss.SeekerId == seeker.guid
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateSeekerSkills_FieldRequiredException()
    {
                var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "abc") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        await Assert.ThrowsAsync<FieldRequiredException>(() => _service.UpdateSeekerSkills("abc", null,principal));
        await Assert.ThrowsAsync<FieldRequiredException>(() => _service.UpdateSeekerSkills("abc", new List<SkillRegisterDto>(),principal));
    }

    [Fact]
    public async Task UpdateSeekerSkills_UserNotFound()
    {
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());
                var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "abc") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.UpdateSeekerSkills("abc", new List<SkillRegisterDto> { new() { Name = "C#" } },principal));
    }

    [Fact]
    public async Task UpdateSeekerSkills_SeekerNotFound()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "abc", PasswordHash = new byte[] { 1 }, HashKey = new byte[] { 2 }, Role = "Seeker" };
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "abc") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.UpdateSeekerSkills("abc", new List<SkillRegisterDto> { new() { Name = "C#" } },principal));
    }

    [Fact]
    public async Task UpdateSeekerSkills_SkillNotFound()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "abc", PasswordHash = new byte[] { 1 }, HashKey = new byte[] { 2 }, Role = "Seeker" };
        var seeker = new Seeker { guid = Guid.NewGuid(), UserId = user.guid };

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "abc") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker> { seeker });
        _skillRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Skill>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.UpdateSeekerSkills("abc", new List<SkillRegisterDto> { new() { Name = "C#" } }, principal));

    }

    [Fact]
    public async Task GetSeekerByUsername_Success()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "muthu@gmail.com", PasswordHash = new byte[] { 1 }, HashKey = new byte[] { 2 }, Role = "Seeker" };
        var seeker = new Seeker { guid = Guid.NewGuid(), FirstName = "Muthu", UserId = user.guid, seekerSkills = new List<SeekerSkills> { new() { skill = new Skill { Name = "C#" } } } };
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker> { seeker });

        var result = await _service.GetSeekerByUsername("muthu@gmail.com");

        Assert.Equal(seeker.FirstName, result.FirstName);
    }

    [Fact]
    public async Task GetSeekerByUsername_UserNotFound()
    {
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.GetSeekerByUsername("abc"));
    }

    [Fact]
    public async Task GetSeekerByUsername_SeekerNotFound()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "abc", PasswordHash = new byte[] { 1 }, HashKey = new byte[] { 2 }, Role = "Seeker" };
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.GetSeekerByUsername("abc"));
    }

    [Fact]
    public async Task SoftDeleteSeekerAsync_Success()
    {
        var userGuid = Guid.NewGuid();
        var user = new User { guid = userGuid, Username = "abc", IsDeleted = true };
        var seeker = new Seeker { guid = Guid.NewGuid(), UserId = userGuid };

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "abc") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker> { seeker });
        _seekerRepoMock.Setup(x => x.Update(seeker.guid, It.IsAny<Seeker>())).ReturnsAsync(seeker);

        await _service.SoftDeleteSeekerAsync("abc", principal);

        _seekerRepoMock.Verify(x => x.Update(seeker.guid, It.Is<Seeker>(s => s.IsDeleted)), Times.Once);
    }

    [Fact]
    public async Task SoftDeleteSeekerAsync_UserNotFound()
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "abc") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.SoftDeleteSeekerAsync("abc", principal));
    }

    [Fact]
    public async Task SoftDeleteSeekerAsync_SeekerNotFound()
    {
        var userGuid = Guid.NewGuid();
        var user = new User { guid = userGuid, Username = "abc", IsDeleted = true };
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "abc") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.SoftDeleteSeekerAsync("abc", principal));
    }

    [Fact]
    public async Task SoftDeleteSeekerAsync_UnauthorizedAccess()
    {
        var userGuid = Guid.NewGuid();
        var user = new User { guid = userGuid, Username = "abc", IsDeleted = true };
        var seeker = new Seeker { guid = Guid.NewGuid(), UserId = userGuid };

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "notabc") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker> { seeker });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.SoftDeleteSeekerAsync("abc", principal));
    }
}