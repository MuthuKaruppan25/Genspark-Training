using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;

public class SkillsServiceTests
{
    private readonly Mock<IRepository<Guid, Skill>> _repoMock = new();
    private readonly SkillsService _service;

    public SkillsServiceTests()
    {
        _service = new SkillsService(_repoMock.Object);
    }

    [Fact]
    public async Task AddSkill_Success()
    {
        var dto = new SkillRegisterDto { Name = "C#" };
        _repoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Skill>());
        _repoMock.Setup(x => x.Add(It.IsAny<Skill>())).ReturnsAsync((Skill s) => s);

        var result = await _service.AddSkill(dto);

        Assert.Equal("C#", result.Name);
    }

    [Fact]
    public async Task AddSkill_ThrowsFieldRequiredException_WhenNameIsEmpty()
    {
        var dto = new SkillRegisterDto { Name = "" };

        await Assert.ThrowsAsync<FieldRequiredException>(() => _service.AddSkill(dto));
    }

    [Fact]
    public async Task AddSkill_ThrowsDuplicateEntryException_WhenNameExists()
    {
        var dto = new SkillRegisterDto { Name = "C#" };
        var existing = new List<Skill> { new Skill { Name = "C#" } };
        _repoMock.Setup(x => x.GetAll()).ReturnsAsync(existing);

        await Assert.ThrowsAsync<DuplicateEntryException>(() => _service.AddSkill(dto));
    }

    [Fact]
    public async Task AddSkill_ThrowsRegistrationException_OnRegistrationException()
    {
        var dto = new SkillRegisterDto { Name = "C#" };
        _repoMock.Setup(x => x.GetAll()).ThrowsAsync(new RegistrationException("db error"));

        await Assert.ThrowsAsync<RegistrationException>(() => _service.AddSkill(dto));
    }

    [Fact]
    public async Task AddSkill_ThrowsException_OnGeneralError()
    {
        var dto = new SkillRegisterDto { Name = "C#" };
        _repoMock.Setup(x => x.GetAll()).ThrowsAsync(new Exception("db error"));

        await Assert.ThrowsAsync<Exception>(() => _service.AddSkill(dto));
    }
}