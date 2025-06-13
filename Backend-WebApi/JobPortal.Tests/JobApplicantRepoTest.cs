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
    public class JobApplicantRepositoryTests : IDisposable
    {
        private readonly JobContext _context;
        private readonly JobApplicantRepository _repository;

        public JobApplicantRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<JobContext>()
                .UseInMemoryDatabase("JobApplicantTestDb")
                .Options;

            _context = new JobContext(options);
            _repository = new JobApplicantRepository(_context);
        }

        [Fact]
        public async Task AddJobApplication_ShouldSaveSuccessfully()
        {
            var company = new Company { CompanyName = "TestCo" };
            await _context.companies.AddAsync(company);

            var user = new User { Username = "seeker@mail.com", Role = "Seeker" };
            await _context.users.AddAsync(user);

            var recruiter = new Recruiter
            {
                FirstName = "Rec",
                LastName = "User",
                UserId = Guid.NewGuid(),
                CompanyId = company.guid,
                company = company
            };
            await _context.recruiters.AddAsync(recruiter);

            var jobPost = new JobPost
            {
                Title = "Backend Developer",
                RecruiterID = recruiter.guid,
                recruiter = recruiter,
                requiredSkills = new List<PostSkills>(),
                responsibilities = new List<Responsibilities>(),
                requirements = new List<Requirements>(),
                jobApplications = new List<JobApplication>(),
                IsDeleted = false
            };
            await _context.jobPosts.AddAsync(jobPost);

            var seeker = new Seeker
            {
                UserId = user.guid,
                jobApplications = new List<JobApplication>()
            };
            await _context.seekers.AddAsync(seeker);

            await _context.SaveChangesAsync();

            var jobApplication = new JobApplication
            {
                SeekerId = seeker.guid,
                seeker = seeker,
                JobPostId = jobPost.guid,
                jobPost = jobPost,
                Status = "Applied",
                IsDeleted = false,
                AppliedOn = DateTime.UtcNow
            };

            await _repository.Add(jobApplication);

            var result = await _repository.Get(jobApplication.guid);

            Assert.NotNull(result);
            Assert.Equal(seeker.guid, result.SeekerId);
            Assert.Equal(jobPost.guid, result.JobPostId);
            Assert.Equal("Applied", result.Status);
        }

        [Fact]
        public async Task Get_InvalidJobApplicationId_ShouldThrow_RecordNotFoundException()
        {
            var ex = await Assert.ThrowsAsync<RecordNotFoundException>(() =>
                _repository.Get(Guid.NewGuid()));

            Assert.Equal("Job Application with the given Id Not Found", ex.Message);
        }

        [Fact]
        public async Task GetAllJobApplications_ShouldReturnJobApplications()
        {
            var company = new Company { CompanyName = "TestCo2" };
            await _context.companies.AddAsync(company);

            var user = new User { Username = "seeker2@mail.com", Role = "Seeker" };
            await _context.users.AddAsync(user);

            var recruiter = new Recruiter
            {
                FirstName = "Rec2",
                LastName = "User2",
                UserId = Guid.NewGuid(),
                CompanyId = company.guid,
                company = company
            };
            await _context.recruiters.AddAsync(recruiter);

            var jobPost = new JobPost
            {
                Title = "Frontend Developer",
                RecruiterID = recruiter.guid,
                recruiter = recruiter,
                requiredSkills = new List<PostSkills>(),
                responsibilities = new List<Responsibilities>(),
                requirements = new List<Requirements>(),
                jobApplications = new List<JobApplication>(),
                IsDeleted = false
            };
            await _context.jobPosts.AddAsync(jobPost);

            var seeker = new Seeker
            {
                UserId = user.guid,
                jobApplications = new List<JobApplication>()
            };
            await _context.seekers.AddAsync(seeker);

            await _context.SaveChangesAsync();

            var jobApplication = new JobApplication
            {
                SeekerId = seeker.guid,
                seeker = seeker,
                JobPostId = jobPost.guid,
                jobPost = jobPost,
                Status = "Applied",
                IsDeleted = false,
                AppliedOn = DateTime.UtcNow
            };

            await _repository.Add(jobApplication);

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