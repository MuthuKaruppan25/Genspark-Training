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
using System.Collections.Generic;

public class JobApplicantControllerTests
{
    private readonly Mock<IJobApplicantService> _jobApplicantServiceMock = new();
    private readonly JobApplicantController _controller;

    public JobApplicantControllerTests()
    {
        _controller = new JobApplicantController(_jobApplicantServiceMock.Object);
    }

    [Fact]
    public async Task CreateApplication_ReturnsOk_WhenSuccess()
    {
        var dto = new JobApplicantAddDto();
        var response = new JobApplicantAddResponse();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
        new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com")
        }, "mock"));

        _jobApplicantServiceMock.Setup(s => s.CreateApplication(dto, user)).ReturnsAsync(response);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.CreateApplication(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task CreateApplication_ReturnsBadRequest_WhenNull()
    {
        var result = await _controller.CreateApplication(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Application data is required.", badRequest.Value);
    }

    [Fact]
    public async Task CreateApplication_ReturnsNotFound_WhenRecordNotFound()
    {
        var dto = new JobApplicantAddDto();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
        new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com")
        }, "mock"));

        _jobApplicantServiceMock.Setup(s => s.CreateApplication(dto, user))
            .ThrowsAsync(new RecordNotFoundException("not found"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.CreateApplication(dto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreateApplication_Returns500_WhenRegistrationException()
    {
        var dto = new JobApplicantAddDto();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
        new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com")
        }, "mock"));

        _jobApplicantServiceMock.Setup(s => s.CreateApplication(dto, user))
            .ThrowsAsync(new RegistrationException("fail"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.CreateApplication(dto);

        Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, ((ObjectResult)result).StatusCode);
    }

    [Fact]
    public async Task GetPagedApplications_ReturnsOk_WhenSuccess()
    {
        var pageDto = new PageDataDto { pageNumber = 1, pageSize = 10 };
        var pagedResult = new PagedResult<JobApplicationDetailsDto>
        {
            Items = new List<JobApplicationDetailsDto>(),
            TotalCount = 0
        };

        _jobApplicantServiceMock
            .Setup(s => s.GetPagedApplications(1, 10))
            .ReturnsAsync(pagedResult);

        var result = await _controller.GetPagedApplications(pageDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(pagedResult, okResult.Value);
    }

    [Fact]
    public async Task GetPagedApplications_ReturnsBadRequest_WhenNull()
    {
        var result = await _controller.GetPagedApplications(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Pagination data is required.", badRequest.Value);
    }

    [Fact]
    public async Task GetPagedApplications_Returns500_WhenFetchDataException()
    {
        var pageDto = new PageDataDto { pageNumber = 1, pageSize = 10 };
        _jobApplicantServiceMock.Setup(s => s.GetPagedApplications(1, 10))
            .ThrowsAsync(new FetchDataException("Fetch error"));

        var result = await _controller.GetPagedApplications(pageDto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Fetch error", errorValue);
    }

    [Fact]
    public async Task SoftDeleteApplication_ReturnsOk_WhenSuccess()
    {
        var appId = Guid.NewGuid();
        _jobApplicantServiceMock.Setup(s => s.SoftDeleteApplication(appId, It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(true);

        // Mock User property
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "seeker@mail.com")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.SoftDeleteApplication(appId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var successObj = okResult.Value;
        Assert.NotNull(successObj);
        var successValue = (bool?)successObj.GetType().GetProperty("success")?.GetValue(successObj);
        Assert.True(successValue.HasValue && successValue.Value);
    }

    [Fact]
    public async Task SoftDeleteApplication_ReturnsNotFound_WhenRecordNotFound()
    {
        var appId = Guid.NewGuid();
        _jobApplicantServiceMock.Setup(s => s.SoftDeleteApplication(appId, It.IsAny<ClaimsPrincipal>()))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.SoftDeleteApplication(appId);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task SoftDeleteApplication_Returns500_WhenUpdateException()
    {
        var appId = Guid.NewGuid();
        _jobApplicantServiceMock.Setup(s => s.SoftDeleteApplication(appId, It.IsAny<ClaimsPrincipal>()))
            .ThrowsAsync(new UpdateException("Update error"));

        var result = await _controller.SoftDeleteApplication(appId);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Update error", errorValue);
    }
}