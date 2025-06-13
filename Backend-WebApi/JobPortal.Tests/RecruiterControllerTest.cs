using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using JobPortal.Controllers;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models.DTOs;
using JobPortal.Models;

public class RecruiterControllerTests
{
    private readonly Mock<IRecruiterService> _recruiterServiceMock = new();
    private readonly RecruiterController _controller;

    public RecruiterControllerTests()
    {
        _controller = new RecruiterController(_recruiterServiceMock.Object);
    }

    [Fact]
    public async Task RegisterRecruiter_ReturnsOk_WhenSuccess()
    {
        var dto = new RecruiterRegisterDto();
        var response = new RecruiterRegisterResponseDto();

        _recruiterServiceMock.Setup(s => s.RegisterCompany(dto)).ReturnsAsync(response);

        var result = await _controller.RegisterRecruiter(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task RegisterRecruiter_ReturnsBadRequest_WhenNull()
    {
        var result = await _controller.RegisterRecruiter(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Recruiter registration data is required.", badRequest.Value);
    }

    [Fact]
    public async Task RegisterRecruiter_ReturnsConflict_WhenDuplicateEntry()
    {
        var dto = new RecruiterRegisterDto();
        _recruiterServiceMock.Setup(s => s.RegisterCompany(dto))
            .ThrowsAsync(new DuplicateEntryException("Duplicate"));

        var result = await _controller.RegisterRecruiter(dto);

        var conflict = Assert.IsType<ConflictObjectResult>(result);
        var errorObj = conflict.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Duplicate", errorValue);
    }

    [Fact]
    public async Task RegisterRecruiter_ReturnsBadRequest_WhenFieldRequired()
    {
        var dto = new RecruiterRegisterDto();
        _recruiterServiceMock.Setup(s => s.RegisterCompany(dto))
            .ThrowsAsync(new FieldRequiredException("Field required"));

        var result = await _controller.RegisterRecruiter(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorObj = badRequest.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Field required", errorValue);
    }

    [Fact]
    public async Task RegisterRecruiter_ReturnsNotFound_WhenRecordNotFound()
    {
        var dto = new RecruiterRegisterDto();
        _recruiterServiceMock.Setup(s => s.RegisterCompany(dto))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.RegisterRecruiter(dto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task RegisterRecruiter_Returns500_WhenRegistrationException()
    {
        var dto = new RecruiterRegisterDto();
        _recruiterServiceMock.Setup(s => s.RegisterCompany(dto))
            .ThrowsAsync(new RegistrationException("Reg error"));

        var result = await _controller.RegisterRecruiter(dto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Reg error", errorValue);
    }

    [Fact]
    public async Task GetRecruiterById_ReturnsOk_WhenSuccess()
    {
        var recruiterId = Guid.NewGuid();
        var recruiter = new Recruiter();

        _recruiterServiceMock.Setup(s => s.GetRecruiterById(recruiterId)).ReturnsAsync(recruiter);

        var result = await _controller.GetRecruiterById(recruiterId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(recruiter, okResult.Value);
    }

    [Fact]
    public async Task GetRecruiterById_ReturnsNotFound_WhenRecordNotFound()
    {
        var recruiterId = Guid.NewGuid();
        _recruiterServiceMock.Setup(s => s.GetRecruiterById(recruiterId))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.GetRecruiterById(recruiterId);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task GetRecruiterById_Returns500_WhenFetchDataException()
    {
        var recruiterId = Guid.NewGuid();
        _recruiterServiceMock.Setup(s => s.GetRecruiterById(recruiterId))
            .ThrowsAsync(new FetchDataException("Fetch error"));

        var result = await _controller.GetRecruiterById(recruiterId);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Fetch error", errorValue);
    }

    [Fact]
    public async Task GetRecruiterJobPosts_ReturnsOk_WhenSuccess()
    {
        var recruiterId = Guid.NewGuid();
        var posts = new List<JobPostDto>();

        _recruiterServiceMock.Setup(s => s.GetRecruiterJobPosts(recruiterId)).ReturnsAsync(posts);

        var result = await _controller.GetRecruiterJobPosts(recruiterId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(posts, okResult.Value);
    }

    [Fact]
    public async Task GetRecruiterJobPosts_Returns500_WhenFetchDataException()
    {
        var recruiterId = Guid.NewGuid();
        _recruiterServiceMock.Setup(s => s.GetRecruiterJobPosts(recruiterId))
            .ThrowsAsync(new FetchDataException("Fetch error"));

        var result = await _controller.GetRecruiterJobPosts(recruiterId);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Fetch error", errorValue);
    }

    [Fact]
    public async Task GetRecruiterByUsername_ReturnsOk_WhenSuccess()
    {
        var recruiter = new Recruiter();
        _recruiterServiceMock.Setup(s => s.GetRecruiterByUsername("user")).ReturnsAsync(recruiter);

        var result = await _controller.GetRecruiterByUsername("user");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(recruiter, okResult.Value);
    }

    [Fact]
    public async Task GetRecruiterByUsername_ReturnsNotFound_WhenRecordNotFound()
    {
        _recruiterServiceMock.Setup(s => s.GetRecruiterByUsername("user"))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.GetRecruiterByUsername("user");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task GetRecruiterByUsername_Returns500_WhenFetchDataException()
    {
        _recruiterServiceMock.Setup(s => s.GetRecruiterByUsername("user"))
            .ThrowsAsync(new FetchDataException("Fetch error"));

        var result = await _controller.GetRecruiterByUsername("user");

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Fetch error", errorValue);
    }

    [Fact]
    public async Task UpdateRecruiterDetails_ReturnsOk_WhenSuccess()
    {
        var recruiterId = Guid.NewGuid();
        var updateDto = new RecruiterUpdateDto();

        _recruiterServiceMock.Setup(s => s.UpdateRecruiterDetails(recruiterId, updateDto)).Returns(Task.CompletedTask);

        var result = await _controller.UpdateRecruiterDetails(recruiterId, updateDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var msgObj = okResult.Value;
        Assert.NotNull(msgObj);
        var msgValue = msgObj.GetType().GetProperty("message")?.GetValue(msgObj)?.ToString();
        Assert.Equal("Recruiter details updated successfully.", msgValue);
    }

    [Fact]
    public async Task UpdateRecruiterDetails_ReturnsBadRequest_WhenNull()
    {
        var recruiterId = Guid.NewGuid();
        var result = await _controller.UpdateRecruiterDetails(recruiterId, null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Update data is required.", badRequest.Value);
    }

    [Fact]
    public async Task UpdateRecruiterDetails_ReturnsNotFound_WhenRecordNotFound()
    {
        var recruiterId = Guid.NewGuid();
        var updateDto = new RecruiterUpdateDto();
        _recruiterServiceMock.Setup(s => s.UpdateRecruiterDetails(recruiterId, updateDto))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.UpdateRecruiterDetails(recruiterId, updateDto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task UpdateRecruiterDetails_Returns500_WhenUpdateException()
    {
        var recruiterId = Guid.NewGuid();
        var updateDto = new RecruiterUpdateDto();
        _recruiterServiceMock.Setup(s => s.UpdateRecruiterDetails(recruiterId, updateDto))
            .ThrowsAsync(new UpdateException("Update error"));

        var result = await _controller.UpdateRecruiterDetails(recruiterId, updateDto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Update error", errorValue);
    }

    [Fact]
    public async Task SoftDeleteRecruiter_ReturnsOk_WhenSuccess()
    {
        var recruiterId = Guid.NewGuid();
        _recruiterServiceMock.Setup(s => s.SoftDeleteRecruiter(recruiterId, It.IsAny<ClaimsPrincipal>()))
            .Returns(Task.CompletedTask);

        // Mock User property
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "recruiter@mail.com")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.SoftDeleteRecruiter(recruiterId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var msgObj = okResult.Value;
        Assert.NotNull(msgObj);
        var msgValue = msgObj.GetType().GetProperty("message")?.GetValue(msgObj)?.ToString();
        Assert.Equal("Recruiter deleted successfully.", msgValue);
    }

    [Fact]
    public async Task SoftDeleteRecruiter_ReturnsNotFound_WhenRecordNotFound()
    {
        var recruiterId = Guid.NewGuid();
        _recruiterServiceMock.Setup(s => s.SoftDeleteRecruiter(recruiterId, It.IsAny<ClaimsPrincipal>()))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.SoftDeleteRecruiter(recruiterId);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task SoftDeleteRecruiter_Returns500_WhenUpdateException()
    {
        var recruiterId = Guid.NewGuid();
        _recruiterServiceMock.Setup(s => s.SoftDeleteRecruiter(recruiterId, It.IsAny<ClaimsPrincipal>()))
            .ThrowsAsync(new UpdateException("Update error"));

        var result = await _controller.SoftDeleteRecruiter(recruiterId);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Update error", errorValue);
    }
}