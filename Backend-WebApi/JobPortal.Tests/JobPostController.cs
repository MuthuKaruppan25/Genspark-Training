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
using JobPortal.Models;

public class JobPostControllerTests
{
    private readonly Mock<IJobPostService> _jobPostServiceMock = new();
    private readonly JobPostController _controller;

    public JobPostControllerTests()
    {
        _controller = new JobPostController(_jobPostServiceMock.Object);
    }

    [Fact]
    public async Task AddJobPost_ReturnsOk_WhenSuccess()
    {
        var dto = new JobPostDto();
        var response = new JobPostRegisterResponseDto();

        _jobPostServiceMock.Setup(s => s.AddJobPost(dto)).ReturnsAsync(response);

        var result = await _controller.AddJobPost(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task AddJobPost_ReturnsBadRequest_WhenNull()
    {
        var result = await _controller.AddJobPost(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Job post data is required.", badRequest.Value);
    }

    [Fact]
    public async Task AddJobPost_ReturnsNotFound_WhenRecordNotFound()
    {
        var dto = new JobPostDto();
        _jobPostServiceMock.Setup(s => s.AddJobPost(dto))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.AddJobPost(dto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task AddJobPost_ReturnsBadRequest_WhenFieldRequired()
    {
        var dto = new JobPostDto();
        _jobPostServiceMock.Setup(s => s.AddJobPost(dto))
            .ThrowsAsync(new FieldRequiredException("Field required"));

        var result = await _controller.AddJobPost(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorObj = badRequest.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Field required", errorValue);
    }

    [Fact]
    public async Task AddJobPost_Returns500_WhenRegistrationException()
    {
        var dto = new JobPostDto();
        _jobPostServiceMock.Setup(s => s.AddJobPost(dto))
            .ThrowsAsync(new RegistrationException("Reg error"));

        var result = await _controller.AddJobPost(dto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Reg error", errorValue);
    }

    [Fact]
    public async Task GetPagedJobPosts_ReturnsOk_WhenSuccess()
    {
        var pageDto = new PageDataDto { pageNumber = 1, pageSize = 10 };
        var pagedResult = new PagedResult<JobPostDto>();

        _jobPostServiceMock.Setup(s => s.GetPagedJobPosts(pageDto)).ReturnsAsync(pagedResult);

        var result = await _controller.GetPagedJobPosts(pageDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(pagedResult, okResult.Value);
    }

    [Fact]
    public async Task GetPagedJobPosts_ReturnsBadRequest_WhenNull()
    {
        var result = await _controller.GetPagedJobPosts(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Pagination data is required.", badRequest.Value);
    }

    [Fact]
    public async Task GetPagedJobPosts_Returns500_WhenFetchDataException()
    {
        var pageDto = new PageDataDto { pageNumber = 1, pageSize = 10 };
        _jobPostServiceMock.Setup(s => s.GetPagedJobPosts(pageDto))
            .ThrowsAsync(new FetchDataException("Fetch error"));

        var result = await _controller.GetPagedJobPosts(pageDto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Fetch error", errorValue);
    }

    [Fact]
    public async Task GetJobPostsMatchingProfile_ReturnsOk_WhenSuccess()
    {
        var pageDto = new PageDataDto { pageNumber = 1, pageSize = 10 };
        var pagedResult = new PagedResult<JobPostDto>
        {
            Items = new List<JobPostDto>(),
            TotalCount = 0
        };
        var guid = Guid.NewGuid();
        _jobPostServiceMock.Setup(s => s.GetJobPostsMatchingProfile(guid, 1, 10)).ReturnsAsync(pagedResult);

        var result = await _controller.GetJobPostsMatchingProfile(guid, pageDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(pagedResult, okResult.Value);
    }


    [Fact]
    public async Task GetJobPostsMatchingProfile_Returns500_WhenFetchDataException()
    {
        var pageDto = new PageDataDto { pageNumber = 1, pageSize = 10 };
         var guid = Guid.NewGuid();
        _jobPostServiceMock.Setup(s => s.GetJobPostsMatchingProfile(guid, 1, 10))
            .ThrowsAsync(new FetchDataException("Fetch error"));

        var result = await _controller.GetJobPostsMatchingProfile(guid, pageDto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Fetch error", errorValue);
    }
    [Fact]
    public async Task GetJobPostsByCompanyName_ReturnsOk_WhenSuccess()
    {
        var companyName = "TestCo";
        var jobPosts = new List<JobPostDto>();
        _jobPostServiceMock.Setup(s => s.GetJobPostsByCompanyNameAsync(companyName)).ReturnsAsync(jobPosts);

        var result = await _controller.GetJobPostsByCompanyName(companyName);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(jobPosts, okResult.Value);
    }

    [Fact]
    public async Task GetJobPostsByCompanyName_Returns500_WhenFetchDataException()
    {
        var companyName = "TestCo";
        _jobPostServiceMock.Setup(s => s.GetJobPostsByCompanyNameAsync(companyName))
            .ThrowsAsync(new FetchDataException("Fetch error"));

        var result = await _controller.GetJobPostsByCompanyName(companyName);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Fetch error", errorValue);
    }

    [Fact]
    public async Task GetJobPostById_ReturnsOk_WhenSuccess()
    {
        var postId = Guid.NewGuid();
        var jobPost = new JobPostDto();
        _jobPostServiceMock.Setup(s => s.GetJobPostByIdAsync(postId)).ReturnsAsync(jobPost);

        var result = await _controller.GetJobPostById(postId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(jobPost, okResult.Value);
    }

    [Fact]
    public async Task GetJobPostById_ReturnsNotFound_WhenRecordNotFound()
    {
        var postId = Guid.NewGuid();
        _jobPostServiceMock.Setup(s => s.GetJobPostByIdAsync(postId))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.GetJobPostById(postId);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task GetJobPostById_Returns500_WhenFetchDataException()
    {
        var postId = Guid.NewGuid();
        _jobPostServiceMock.Setup(s => s.GetJobPostByIdAsync(postId))
            .ThrowsAsync(new FetchDataException("Fetch error"));

        var result = await _controller.GetJobPostById(postId);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Fetch error", errorValue);
    }



    [Fact]
    public async Task GetJobPostWithPagedApplicants_ReturnsBadRequest_WhenNull()
    {
        var postId = Guid.NewGuid();
        var result = await _controller.GetJobPostWithPagedApplicants(postId, null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Pagination data is required.", badRequest.Value);
    }



    

    [Fact]
    public async Task SoftDeleteJobPost_ReturnsOk_WhenSuccess()
    {
        var postId = Guid.NewGuid();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
        new Claim(ClaimTypes.NameIdentifier, "rec@mail.com")
        }, "mock"));

        _jobPostServiceMock.Setup(s => s.SoftDeleteJobPost(postId, user)).ReturnsAsync(true);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.SoftDeleteJobPost(postId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value.GetType().GetProperty("success")?.GetValue(okResult.Value));
    }

    [Fact]
    public async Task SoftDeleteJobPost_ReturnsNotFound_WhenRecordNotFound()
    {
        var postId = Guid.NewGuid();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
        new Claim(ClaimTypes.NameIdentifier, "rec@mail.com")
        }, "mock"));

        _jobPostServiceMock.Setup(s => s.SoftDeleteJobPost(postId, user))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.SoftDeleteJobPost(postId);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task SoftDeleteJobPost_Returns500_WhenUpdateException()
    {
        var postId = Guid.NewGuid();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
        new Claim(ClaimTypes.NameIdentifier, "rec@mail.com")
        }, "mock"));

        _jobPostServiceMock.Setup(s => s.SoftDeleteJobPost(postId, user))
            .ThrowsAsync(new UpdateException("Update error"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.SoftDeleteJobPost(postId);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Update error", errorValue);
    }

    [Fact]
    public async Task UpdateJobPost_ReturnsOk_WhenSuccess()
    {
        var postId = Guid.NewGuid();
        var updateDto = new JobPostUpdateDto();
        var response = new JobPostDto();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
        new Claim(ClaimTypes.NameIdentifier, "rec@mail.com")
        }, "mock"));

        _jobPostServiceMock.Setup(s => s.UpdateJobPost(postId, updateDto, user)).ReturnsAsync(response);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.UpdateJobPost(postId, updateDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task UpdateJobPost_ReturnsNotFound_WhenRecordNotFound()
    {
        var postId = Guid.NewGuid();
        var updateDto = new JobPostUpdateDto();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
        new Claim(ClaimTypes.NameIdentifier, "rec@mail.com")
        }, "mock"));

        _jobPostServiceMock.Setup(s => s.UpdateJobPost(postId, updateDto, user))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.UpdateJobPost(postId, updateDto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task UpdateJobPost_Returns500_WhenUpdateException()
    {
        var postId = Guid.NewGuid();
        var updateDto = new JobPostUpdateDto();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
        new Claim(ClaimTypes.NameIdentifier, "rec@mail.com")
        }, "mock"));

        _jobPostServiceMock.Setup(s => s.UpdateJobPost(postId, updateDto, user))
            .ThrowsAsync(new UpdateException("Update error"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.UpdateJobPost(postId, updateDto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Update error", errorValue);
    }
}