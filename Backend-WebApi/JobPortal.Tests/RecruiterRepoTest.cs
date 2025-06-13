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
    public class RecruiterRepositoryTests : IDisposable
    {
        private readonly JobContext _context;
        private readonly RecruiterRepository _repository;

        public RecruiterRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<JobContext>()
                .UseInMemoryDatabase("RecruiterTestDb")
                .Options;

            _context = new JobContext(options);
            _repository = new RecruiterRepository(_context);
        }

        [Fact]
        public async Task AddRecruiter_WithValidUserAndCompany_ShouldSaveSuccessfully()
        {
            var user = new User
            {
                Username = "recruiter@example.com",
                PasswordHash = new byte[] { 1, 2, 3 },
                HashKey = new byte[] { 4, 5, 6 },
                Role = "Recruiter"
            };
            var company = new Company
            {
                CompanyName = "TestCo"
            };

            await _context.users.AddAsync(user);
            await _context.companies.AddAsync(company);
            await _context.SaveChangesAsync();

            var recruiter = new Recruiter
            {
                FirstName = "Alice",
                LastName = "Smith",
                PhoneNumber = "1234567890",
                Designation = "HR",
                UserId = user.guid,
                CompanyId = company.guid
            };

            await _repository.Add(recruiter);

            var result = await _repository.Get(recruiter.guid);

            Assert.NotNull(result);
            Assert.Equal("Alice", result.FirstName);
            Assert.Equal(user.guid, result.UserId);
            Assert.Equal(company.guid, result.CompanyId);
        }

        [Fact]
        public async Task Get_InvalidRecruiterId_ShouldThrow_RecordNotFoundException()
        {
            var ex = await Assert.ThrowsAsync<RecordNotFoundException>(() =>
                _repository.Get(Guid.NewGuid()));

            Assert.Equal("Recruiter with the given Id Not Found", ex.Message);
        }

        [Fact]
        public async Task GetAllRecruiters_ShouldReturnRecruiters()
        {
            var user = new User
            {
                Username = "rec@job.com",
                PasswordHash = new byte[] { 10, 20 },
                HashKey = new byte[] { 30, 40 },
                Role = "Recruiter"
            };
            var company = new Company { CompanyName = "GetAllCo" };
            await _context.users.AddAsync(user);
            await _context.companies.AddAsync(company);
            await _context.SaveChangesAsync();

            var recruiter = new Recruiter
            {
                FirstName = "Bob",
                LastName = "Johnson",
                PhoneNumber = "9876543210",
                Designation = "Lead",
                UserId = user.guid,
                CompanyId = company.guid
            };
            await _repository.Add(recruiter);

            var result = await _repository.GetAll();

            Assert.NotEmpty(result);
        
        }

        [Fact]
        public async Task UpdateRecruiter_ShouldUpdateSuccessfully()
        {
            var user = new User
            {
                Username = "update@rec.com",
                PasswordHash = new byte[] { 9 },
                HashKey = new byte[] { 8 },
                Role = "Recruiter"
            };
            var company = new Company { CompanyName = "UpdateCo" };
            await _context.users.AddAsync(user);
            await _context.companies.AddAsync(company);
            await _context.SaveChangesAsync();

            var recruiter = new Recruiter
            {
                FirstName = "Initial",
                LastName = "Rec",
                PhoneNumber = "1112223333",
                Designation = "Old",
                UserId = user.guid,
                CompanyId = company.guid
            };
            await _repository.Add(recruiter);

            recruiter.FirstName = "Updated";
            recruiter.Designation = "Updated Designation";

            var updated = await _repository.Update(recruiter.guid, recruiter);

            Assert.Equal("Updated", updated.FirstName);
            Assert.Equal("Updated Designation", updated.Designation);
        }

        [Fact]
        public async Task UpdateRecruiter_InvalidId_ShouldThrowException()
        {
            var recruiter = new Recruiter
            {
                guid = Guid.NewGuid(),
                FirstName = "Ghost",
                UserId = Guid.NewGuid(),
                CompanyId = Guid.NewGuid()
            };

            var ex = await Assert.ThrowsAsync<RecordNotFoundException>(() =>
                _repository.Update(recruiter.guid, recruiter));

            Assert.Equal("Recruiter with the given Id Not Found", ex.Message);
        }

        [Fact]
        public async Task DeleteRecruiter_ShouldDeleteSuccessfully()
        {
            var user = new User
            {
                Username = "delete@rec.com",
                PasswordHash = new byte[] { 3 },
                HashKey = new byte[] { 2 },
                Role = "Recruiter"
            };
            var company = new Company { CompanyName = "DeleteCo" };
            await _context.users.AddAsync(user);
            await _context.companies.AddAsync(company);
            await _context.SaveChangesAsync();

            var recruiter = new Recruiter
            {
                FirstName = "ToDelete",
                LastName = "Rec",
                UserId = user.guid,
                CompanyId = company.guid
            };
            await _repository.Add(recruiter);

            var deleted = await _repository.Delete(recruiter.guid);

            Assert.Equal(recruiter.guid, deleted.guid);
        }

        [Fact]
        public async Task DeleteRecruiter_InvalidId_ShouldThrowException()
        {
            var ex = await Assert.ThrowsAsync<RecordNotFoundException>(() =>
                _repository.Delete(Guid.NewGuid()));

            Assert.Equal("Recruiter with the given Id Not Found", ex.Message);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}