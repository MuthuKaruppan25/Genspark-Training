using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using JobPortal.Controllers;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models.DTOs;

public class SeekerControllerTests
{
    private readonly Mock<ISeekerService> _seekerServiceMock = new();
    private readonly SeekerController _controller;

    public SeekerControllerTests()
    {
        _controller = new SeekerController(_seekerServiceMock.Object);
    }

    [Fact]
    public async Task RegisterSeeker_ReturnsOk_WhenSuccess()
    {
        var dto = new SeekerRegisterDto();
        var response = new SeekerRegisterResponseDto();

        _seekerServiceMock.Setup(s => s.RegisterSeeker(dto)).ReturnsAsync(response);

        var result = await _controller.RegisterSeeker(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task RegisterSeeker_ReturnsBadRequest_WhenNull()
    {
        var result = await _controller.RegisterSeeker(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Seeker registration data is required.", badRequest.Value);
    }

    [Fact]
    public async Task RegisterSeeker_ReturnsConflict_WhenDuplicateEntry()
    {
        var dto = new SeekerRegisterDto();
        _seekerServiceMock.Setup(s => s.RegisterSeeker(dto))
            .ThrowsAsync(new DuplicateEntryException("Duplicate"));

        var result = await _controller.RegisterSeeker(dto);

        var conflict = Assert.IsType<ConflictObjectResult>(result);
        var errorObj = conflict.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Duplicate", errorValue);
    }

    [Fact]
    public async Task RegisterSeeker_ReturnsBadRequest_WhenFieldRequired()
    {
        var dto = new SeekerRegisterDto();
        _seekerServiceMock.Setup(s => s.RegisterSeeker(dto))
            .ThrowsAsync(new FieldRequiredException("Field required"));

        var result = await _controller.RegisterSeeker(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorObj = badRequest.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Field required", errorValue);
    }

    [Fact]
    public async Task RegisterSeeker_ReturnsNotFound_WhenRecordNotFound()
    {
        var dto = new SeekerRegisterDto();
        _seekerServiceMock.Setup(s => s.RegisterSeeker(dto))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.RegisterSeeker(dto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task RegisterSeeker_Returns500_WhenRegistrationException()
    {
        var dto = new SeekerRegisterDto();
        _seekerServiceMock.Setup(s => s.RegisterSeeker(dto))
            .ThrowsAsync(new RegistrationException("Reg error"));

        var result = await _controller.RegisterSeeker(dto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Reg error", errorValue);
    }

    [Fact]
    public async Task GetPagedSeekers_ReturnsOk_WhenSuccess()
    {
        var pageDto = new PageDataDto { pageNumber = 1, pageSize = 10 };
        var seekers = new List<SeekerRegisterResponseDto>();
        _seekerServiceMock.Setup(s => s.GetPagedSeekers(1, 10)).ReturnsAsync(seekers);

        var result = await _controller.GetPagedSeekers(pageDto);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(seekers, okResult.Value);

    }

    [Fact]
    public async Task GetPagedSeekers_ReturnsBadRequest_WhenNull()
    {
        var result = await _controller.GetPagedSeekers(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Pagination data is required.", badRequest.Value);
    }

    [Fact]
    public async Task GetPagedSeekers_ReturnsNotFound_WhenNoRecordsFound()
    {
        var pageDto = new PageDataDto { pageNumber = 1, pageSize = 10 };
        _seekerServiceMock.Setup(s => s.GetPagedSeekers(1, 10))
            .ThrowsAsync(new NoRecordsFoundException("No records"));

        var result = await _controller.GetPagedSeekers(pageDto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("No records", errorValue);
    }

    [Fact]
    public async Task GetPagedSeekers_Returns500_WhenFetchDataException()
    {
        var pageDto = new PageDataDto { pageNumber = 1, pageSize = 10 };
        _seekerServiceMock.Setup(s => s.GetPagedSeekers(1, 10))
            .ThrowsAsync(new FetchDataException("Fetch error"));

        var result = await _controller.GetPagedSeekers(pageDto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Fetch error", errorValue);
    }

    [Fact]
    public async Task GetSeekerWithApplications_ReturnsOk_WhenSuccess()
    {
        var resultObj = new SeekerWithApplicationsDto();
        _seekerServiceMock
            .Setup(s => s.GetSeekerWithApplications("user"))
            .ReturnsAsync(resultObj);

        var result = await _controller.GetSeekerWithApplications("user");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultObj, okResult.Value);
    }

    [Fact]
    public async Task GetSeekerWithApplications_ReturnsNotFound_WhenRecordNotFound()
    {
        _seekerServiceMock.Setup(s => s.GetSeekerWithApplications("user"))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.GetSeekerWithApplications("user");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task GetSeekerWithApplications_Returns500_WhenFetchDataException()
    {
        _seekerServiceMock.Setup(s => s.GetSeekerWithApplications("user"))
            .ThrowsAsync(new FetchDataException("Fetch error"));

        var result = await _controller.GetSeekerWithApplications("user");

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Fetch error", errorValue);
    }

    [Fact]
    public async Task GetSeekerSkills_ReturnsOk_WhenSuccess()
    {
        var resultObj = new List<SkillRegisterDto>();
        _seekerServiceMock.Setup(s => s.GetSeekerSkills("user")).ReturnsAsync(resultObj);

        var result = await _controller.GetSeekerSkills("user");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultObj, okResult.Value);
    }

    [Fact]
    public async Task GetSeekerSkills_ReturnsNotFound_WhenRecordNotFound()
    {
        _seekerServiceMock.Setup(s => s.GetSeekerSkills("user"))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.GetSeekerSkills("user");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task GetSeekerSkills_Returns500_WhenFetchDataException()
    {
        _seekerServiceMock.Setup(s => s.GetSeekerSkills("user"))
            .ThrowsAsync(new FetchDataException("Fetch error"));

        var result = await _controller.GetSeekerSkills("user");

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Fetch error", errorValue);
    }
   [Fact]
public async Task UpdateSeekerDetails_ReturnsOk_WhenSuccess()
{
    var updateDto = new SeekerUpdateDto();

    _seekerServiceMock.Setup(s => s.UpdateSeekerDetails("user", updateDto, It.IsAny<ClaimsPrincipal>()))
        .Returns(Task.CompletedTask);

    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
    {
        new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com")
    }, "mock"));

    _controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext { User = user }
    };

    var result = await _controller.UpdateSeekerDetails("user", updateDto);

    var okResult = Assert.IsType<OkObjectResult>(result);
    var msgObj = okResult.Value;
    Assert.NotNull(msgObj);
    var msgValue = msgObj.GetType().GetProperty("message")?.GetValue(msgObj)?.ToString();
    Assert.Equal("Seeker details updated successfully.", msgValue);
}

[Fact]
public async Task UpdateSeekerDetails_ReturnsBadRequest_WhenNull()
{
    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
    {
        new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com")
    }, "mock"));

    _controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext { User = user }
    };

    var result = await _controller.UpdateSeekerDetails("user", null);

    var badRequest = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Update data is required.", badRequest.Value);
}

[Fact]
public async Task UpdateSeekerDetails_ReturnsNotFound_WhenRecordNotFound()
{
    var updateDto = new SeekerUpdateDto();
    _seekerServiceMock.Setup(s => s.UpdateSeekerDetails("user", updateDto, It.IsAny<ClaimsPrincipal>()))
        .ThrowsAsync(new RecordNotFoundException("Not found"));

    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
    {
        new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com")
    }, "mock"));

    _controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext { User = user }
    };

    var result = await _controller.UpdateSeekerDetails("user", updateDto);

    var notFound = Assert.IsType<NotFoundObjectResult>(result);
    var errorObj = notFound.Value;
    Assert.NotNull(errorObj);
    var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
    Assert.Equal("Not found", errorValue);
}

[Fact]
public async Task UpdateSeekerDetails_Returns500_WhenUpdateException()
{
    var updateDto = new SeekerUpdateDto();
    _seekerServiceMock.Setup(s => s.UpdateSeekerDetails("user", updateDto, It.IsAny<ClaimsPrincipal>()))
        .ThrowsAsync(new UpdateException("Update error"));

    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
    {
        new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com")
    }, "mock"));

    _controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext { User = user }
    };

    var result = await _controller.UpdateSeekerDetails("user", updateDto);

    var serverError = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, serverError.StatusCode);
    var errorObj = serverError.Value;
    Assert.NotNull(errorObj);
    var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
    Assert.Equal("Update error", errorValue);
}

[Fact]
public async Task UpdateSeekerSkills_ReturnsOk_WhenSuccess()
{
    var skills = new List<SkillRegisterDto> { new SkillRegisterDto() };

    _seekerServiceMock.Setup(s => s.UpdateSeekerSkills("user", skills, It.IsAny<ClaimsPrincipal>())).Returns(Task.CompletedTask);

    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
    {
        new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com")
    }, "mock"));

    _controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext { User = user }
    };

    var result = await _controller.UpdateSeekerSkills("user", skills);

    var okResult = Assert.IsType<OkObjectResult>(result);
    var msgObj = okResult.Value;
    Assert.NotNull(msgObj);
    var msgValue = msgObj.GetType().GetProperty("message")?.GetValue(msgObj)?.ToString();
    Assert.Equal("Seeker skills updated successfully.", msgValue);
}

[Fact]
public async Task UpdateSeekerSkills_ReturnsBadRequest_WhenNullOrEmpty()
{
    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
    {
        new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com")
    }, "mock"));

    _controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext { User = user }
    };

    var result = await _controller.UpdateSeekerSkills("user", null);

    var badRequest = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("At least one skill is required.", badRequest.Value);

    var result2 = await _controller.UpdateSeekerSkills("user", new List<SkillRegisterDto>());

    var badRequest2 = Assert.IsType<BadRequestObjectResult>(result2);
    Assert.Equal("At least one skill is required.", badRequest2.Value);
}

[Fact]
public async Task UpdateSeekerSkills_ReturnsNotFound_WhenRecordNotFound()
{
    var skills = new List<SkillRegisterDto> { new SkillRegisterDto() };
    _seekerServiceMock.Setup(s => s.UpdateSeekerSkills("user", skills, It.IsAny<ClaimsPrincipal>()))
        .ThrowsAsync(new RecordNotFoundException("Not found"));

    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
    {
        new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com")
    }, "mock"));

    _controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext { User = user }
    };

    var result = await _controller.UpdateSeekerSkills("user", skills);

    var notFound = Assert.IsType<NotFoundObjectResult>(result);
    var errorObj = notFound.Value;
    Assert.NotNull(errorObj);
    var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
    Assert.Equal("Not found", errorValue);
}

[Fact]
public async Task UpdateSeekerSkills_Returns500_WhenUpdateException()
{
    var skills = new List<SkillRegisterDto> { new SkillRegisterDto() };
    _seekerServiceMock.Setup(s => s.UpdateSeekerSkills("user", skills, It.IsAny<ClaimsPrincipal>()))
        .ThrowsAsync(new UpdateException("Update error"));

    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
    {
        new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com")
    }, "mock"));

    _controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext { User = user }
    };

    var result = await _controller.UpdateSeekerSkills("user", skills);

    var serverError = Assert.IsType<ObjectResult>(result);
    Assert.Equal(500, serverError.StatusCode);
    var errorObj = serverError.Value;
    Assert.NotNull(errorObj);
    var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
    Assert.Equal("Update error", errorValue);
}
    [Fact]
    public async Task GetSeekerByUsername_ReturnsOk_WhenSuccess()
    {
        var resultObj = new SeekerRegisterResponseDto(); // Use the correct DTO type
        _seekerServiceMock.Setup(s => s.GetSeekerByUsername("user")).ReturnsAsync(resultObj);

        var result = await _controller.GetSeekerByUsername("user");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultObj, okResult.Value);
    }

    [Fact]
    public async Task GetSeekerByUsername_ReturnsNotFound_WhenRecordNotFound()
    {
        _seekerServiceMock.Setup(s => s.GetSeekerByUsername("user"))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.GetSeekerByUsername("user");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task GetSeekerByUsername_Returns500_WhenFetchDataException()
    {
        _seekerServiceMock.Setup(s => s.GetSeekerByUsername("user"))
            .ThrowsAsync(new FetchDataException("Fetch error"));

        var result = await _controller.GetSeekerByUsername("user");

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Fetch error", errorValue);
    }

    [Fact]
    public async Task SoftDeleteSeeker_ReturnsOk_WhenSuccess()
    {
        _seekerServiceMock.Setup(s => s.SoftDeleteSeekerAsync("user", It.IsAny<ClaimsPrincipal>()))
            .Returns(Task.CompletedTask);

        // Mock User property
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.SoftDeleteSeeker("user");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var msgObj = okResult.Value;
        Assert.NotNull(msgObj);
        var msgValue = msgObj.GetType().GetProperty("message")?.GetValue(msgObj)?.ToString();
        Assert.Equal("Seeker deleted successfully.", msgValue);
    }

    [Fact]
    public async Task SoftDeleteSeeker_ReturnsNotFound_WhenRecordNotFound()
    {
        _seekerServiceMock.Setup(s => s.SoftDeleteSeekerAsync("user", It.IsAny<ClaimsPrincipal>()))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.SoftDeleteSeeker("user");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task SoftDeleteSeeker_ReturnsBadRequest_WhenInvalidOperationException()
    {
        _seekerServiceMock.Setup(s => s.SoftDeleteSeekerAsync("user", It.IsAny<ClaimsPrincipal>()))
            .ThrowsAsync(new InvalidOperationException("Invalid op"));

        var result = await _controller.SoftDeleteSeeker("user");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorObj = badRequest.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Invalid op", errorValue);
    }

    [Fact]
    public async Task SoftDeleteSeeker_Returns500_WhenUpdateException()
    {
        _seekerServiceMock.Setup(s => s.SoftDeleteSeekerAsync("user", It.IsAny<ClaimsPrincipal>()))
            .ThrowsAsync(new UpdateException("Update error"));

        var result = await _controller.SoftDeleteSeeker("user");

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Update error", errorValue);
    }
}