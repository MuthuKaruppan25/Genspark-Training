using Xunit;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JobPortal.Contexts;
using JobPortal.Models;
using JobPortal.Repositories;
using JobPortal.Exceptions;

namespace JobPortal.Tests
{
    public class RequirementRepositoryTests : IDisposable
    {
        private readonly JobContext _context;
        private readonly RequirementRepository _repository;

        public RequirementRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<JobContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new JobContext(options);
            _repository = new RequirementRepository(_context);
        }

        [Fact]
        public async Task AddRequirement_ShouldSaveSuccessfully()
        {
            var jobPost = new JobPost { Title = "Test Job" };
            await _context.jobPosts.AddAsync(jobPost);
            await _context.SaveChangesAsync();

            var requirement = new Requirements
            {
                Name = "Test Requirement",
                PostId = jobPost.guid,
                jobPost = jobPost
            };

            await _repository.Add(requirement);

            var result = await _repository.Get(requirement.guid);

            Assert.NotNull(result);
            Assert.Equal("Test Requirement", result.Name);
            Assert.Equal(jobPost.guid, result.PostId);
        }

        [Fact]
        public async Task Get_InvalidRequirementId_ShouldThrow_RecordNotFoundException()
        {
            await Assert.ThrowsAsync<RecordNotFoundException>(() =>
                _repository.Get(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetAllRequirements_ShouldReturnRequirements()
        {
            var jobPost = new JobPost { Title = "Test Job 2" };
            await _context.jobPosts.AddAsync(jobPost);
            await _context.SaveChangesAsync();

            var requirement = new Requirements
            {
                Name = "Requirement 2",
                PostId = jobPost.guid,
                jobPost = jobPost
            };
            await _repository.Add(requirement);

            var result = await _repository.GetAll();

            Assert.NotEmpty(result);
            Assert.Single(result);
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }
}