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
    public class SeekerRepositoryTests : IDisposable
    {
        private readonly JobContext _context;
        private readonly SeekerRepository _repository;

        public SeekerRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<JobContext>()
                .UseInMemoryDatabase("SeekerTestDb")
                .Options;

            _context = new JobContext(options);
            _repository = new SeekerRepository(_context);
        }

        [Fact]
        public async Task AddSeeker_WithValidUser_ShouldSaveSuccessfully()
        {
            var user = new User
            {
                Username = "test@example.com",
                PasswordHash = new byte[] { 1, 2, 3 },
                HashKey = new byte[] { 4, 5, 6 },
                Role = "Seeker"
            };

            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync();

            var seeker = new Seeker
            {
                FirstName = "John",
                LastName = "Doe",
                Experience = 3,
                About = "Full Stack Developer",
                Education = "B.Sc CS",
                UserId = user.guid
            };

            await _repository.Add(seeker);

            var result = await _repository.Get(seeker.guid);

            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
            Assert.Equal(user.guid, result.UserId);
        }

        [Fact]
        public async Task Get_InvalidSeekerId_ShouldThrow_RecordNotFoundException()
        {
            var ex = await Assert.ThrowsAsync<RecordNotFoundException>(() =>
                _repository.Get(Guid.NewGuid()));

            Assert.Equal("Seeker with the given Id Not Found", ex.Message);
        }

        [Fact]
        public async Task GetAllSeekers_ShouldReturnSeekers()
        {
            var user = new User
            {
                Username = "example@job.com",
                PasswordHash = new byte[] { 10, 20 },
                HashKey = new byte[] { 30, 40 },
                Role = "Seeker"
            };
            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync();

            var seeker = new Seeker
            {
                FirstName = "Jane",
                LastName = "Smith",
                Experience = 5,
                About = "Backend Developer",
                Education = "M.Tech",
                UserId = user.guid
            };
            await _repository.Add(seeker);

            var result = await _repository.GetAll();

            Assert.NotEmpty(result);
            Assert.Single(result);
        }



        [Fact]
        public async Task UpdateSeeker_ShouldUpdateSuccessfully()
        {
            var user = new User
            {
                Username = "update@example.com",
                PasswordHash = new byte[] { 9 },
                HashKey = new byte[] { 8 },
                Role = "Seeker"
            };
            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync();

            var seeker = new Seeker
            {
                FirstName = "Initial",
                LastName = "Name",
                Experience = 2,
                About = "Old Info",
                Education = "BCA",
                UserId = user.guid
            };
            await _repository.Add(seeker);

            seeker.FirstName = "Updated";
            seeker.About = "Updated Info";

            var updated = await _repository.Update(seeker.guid, seeker);

            Assert.Equal("Updated", updated.FirstName);
            Assert.Equal("Updated Info", updated.About);
        }

        [Fact]
        public async Task UpdateSeeker_InvalidId_ShouldThrowException()
        {
            var seeker = new Seeker
            {
                guid = Guid.NewGuid(),
                FirstName = "Ghost",
                Experience = 0,
                UserId = Guid.NewGuid()
            };

            var ex = await Assert.ThrowsAsync<RecordNotFoundException>(() =>
                _repository.Update(seeker.guid, seeker));

            Assert.Equal("Seeker with the given Id Not Found", ex.Message);
        }

        [Fact]
        public async Task DeleteSeeker_ShouldDeleteSuccessfully()
        {
            var user = new User
            {
                Username = "delete@example.com",
                PasswordHash = new byte[] { 3 },
                HashKey = new byte[] { 2 },
                Role = "Seeker"
            };
            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync();

            var seeker = new Seeker
            {
                FirstName = "ToDelete",
                LastName = "User",
                Experience = 1,
                UserId = user.guid
            };
            await _repository.Add(seeker);

            var deleted = await _repository.Delete(seeker.guid);

            Assert.Equal(seeker.guid, deleted.guid);
  
        }

        [Fact]
        public async Task DeleteSeeker_InvalidId_ShouldThrowException()
        {
            var ex = await Assert.ThrowsAsync<RecordNotFoundException>(() =>
                _repository.Delete(Guid.NewGuid()));

            Assert.Equal("Seeker with the given Id Not Found", ex.Message);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
