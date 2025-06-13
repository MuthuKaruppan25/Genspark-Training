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
    public class ResponsibilityRepositoryTests : IDisposable
    {
        private readonly JobContext _context;
        private readonly ResponsibilityRepository _repository;

        public ResponsibilityRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<JobContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new JobContext(options);
            _repository = new ResponsibilityRepository(_context);
        }

        [Fact]
        public async Task AddResponsibility_ShouldSaveSuccessfully()
        {
            var jobPost = new JobPost { Title = "Test Job" };
            await _context.jobPosts.AddAsync(jobPost);
            await _context.SaveChangesAsync();

            var responsibility = new Responsibilities
            {
                Name = "Test Responsibility",
                PostId = jobPost.guid,
                jobPost = jobPost
            };

            await _repository.Add(responsibility);

            var result = await _repository.Get(responsibility.guid);

            Assert.NotNull(result);
            Assert.Equal("Test Responsibility", result.Name);
            Assert.Equal(jobPost.guid, result.PostId);
        }

        [Fact]
        public async Task Get_InvalidResponsibilityId_ShouldThrow_RecordNotFoundException()
        {
            await Assert.ThrowsAsync<RecordNotFoundException>(() =>
                _repository.Get(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetAllResponsibilities_ShouldReturnResponsibilities()
        {
            var jobPost = new JobPost { Title = "Test Job 2" };
            await _context.jobPosts.AddAsync(jobPost);
            await _context.SaveChangesAsync();

            var responsibility = new Responsibilities
            {
                Name = "Responsibility 2",
                PostId = jobPost.guid,
                jobPost = jobPost
            };
            await _repository.Add(responsibility);

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