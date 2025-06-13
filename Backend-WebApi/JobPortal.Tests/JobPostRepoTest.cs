using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JobPortal.Contexts;
using JobPortal.Models;
using JobPortal.Repositories;
using JobPortal.Exceptions;

namespace JobPortal.Tests
{
    public class JobPostRepositoryTests : IDisposable
    {
        private readonly JobContext _context;
        private readonly JobPostRepository _repository;

        public JobPostRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<JobContext>()
                .UseInMemoryDatabase("JobPostTestDb")
                .Options;

            _context = new JobContext(options);
            _repository = new JobPostRepository(_context);
        }

        [Fact]
        public async Task AddJobPost_ShouldSaveSuccessfully()
        {
            var company = new Company { CompanyName = "TestCo" };
            await _context.companies.AddAsync(company);
            await _context.SaveChangesAsync();

            var recruiter = new Recruiter
            {
                FirstName = "Rec",
                LastName = "User",
                PhoneNumber = "1234567890",
                Designation = "HR",
                UserId = Guid.NewGuid(),
                CompanyId = company.guid,
                company = company
            };
            await _context.recruiters.AddAsync(recruiter);
            await _context.SaveChangesAsync();

            var jobPost = new JobPost
            {
                Title = "Backend Developer",
                Description = "Develop APIs",
                EmploymentType = "Full-Time",
                EmploymentPosition = "Mid",
                Location = "Remote",
                SalaryPackage = "10LPA",
                RecruiterID = recruiter.guid,
                PostedDate = DateTime.UtcNow,
                LastDate = DateTime.UtcNow.AddDays(30),
                recruiter = recruiter,
                requiredSkills = new List<PostSkills>(),
                responsibilities = new List<Responsibilities>(),
                requirements = new List<Requirements>(),
                jobApplications = new List<JobApplication>(),
                IsDeleted = false
            };

            await _repository.Add(jobPost);

            var result = await _repository.Get(jobPost.guid);

            Assert.NotNull(result);
            Assert.Equal("Backend Developer", result.Title);
            Assert.Equal(recruiter.guid, result.RecruiterID);
        }


        [Fact]
        public async Task Get_InvalidJobPostId_ShouldThrow_RecordNotFoundException()
        {
            var ex = await Assert.ThrowsAsync<RecordNotFoundException>(() =>
                _repository.Get(Guid.NewGuid()));

            Assert.Equal("Job Post with the given Id Not Found", ex.Message);
        }

        [Fact]
        public async Task GetAllJobPosts_ShouldReturnJobPosts()
        {
            var company = new Company { CompanyName = "TestCo2" };
            await _context.companies.AddAsync(company);
            await _context.SaveChangesAsync();

            var recruiter = new Recruiter
            {
                FirstName = "Rec2",
                LastName = "User2",
                PhoneNumber = "9876543210",
                Designation = "Lead",
                UserId = Guid.NewGuid(),
                CompanyId = company.guid,
                company = company
            };
            await _context.recruiters.AddAsync(recruiter);
            await _context.SaveChangesAsync();

            var jobPost = new JobPost
            {
                Title = "Backend Developer",
                Description = "Develop APIs",
                EmploymentType = "Full-Time",
                EmploymentPosition = "Mid",
                Location = "Remote",
                SalaryPackage = "10LPA",
                RecruiterID = recruiter.guid,
                PostedDate = DateTime.UtcNow,
                LastDate = DateTime.UtcNow.AddDays(30),
                recruiter = recruiter,
                requiredSkills = new List<PostSkills>(),
                responsibilities = new List<Responsibilities>(),
                requirements = new List<Requirements>(),
                jobApplications = new List<JobApplication>(),
                IsDeleted = false
            };

            await _repository.Add(jobPost);

            var result = await _repository.GetAll();

            Assert.NotEmpty(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}