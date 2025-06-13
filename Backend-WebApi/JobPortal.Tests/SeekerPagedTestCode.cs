using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JobPortal.Contexts;
using JobPortal.Models;

namespace JobPortal.Tests
{
    public class SeekerPagedGetTests : IDisposable
    {
        private readonly JobContext _context;
        private readonly SeekerPagedGet _service;

        public SeekerPagedGetTests()
        {
            var options = new DbContextOptionsBuilder<JobContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            _context = new JobContext(options);
            _service = new SeekerPagedGet(_context);
        }

        [Fact]
        public async Task GetPaged_ReturnsPagedSeekers()
        {
            // Arrange
            var user1 = new User { Username = "user1@mail.com" };
            var user2 = new User { Username = "user2@mail.com" };
            var user3 = new User { Username = "user3@mail.com" };
            await _context.users.AddRangeAsync(user1, user2, user3);

            var seeker1 = new Seeker { FirstName = "Alice", UserId = user1.guid, user = user1, IsDeleted = false };
            var seeker2 = new Seeker { FirstName = "Bob", UserId = user2.guid, user = user2, IsDeleted = false };
            var seeker3 = new Seeker { FirstName = "Charlie", UserId = user3.guid, user = user3, IsDeleted = false };
            await _context.seekers.AddRangeAsync(seeker1, seeker2, seeker3);

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetPaged(1, 2);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, s => s.FirstName == "Alice");
            Assert.Contains(result, s => s.FirstName == "Bob");
        }

        [Fact]
        public async Task GetPaged_SkipsAndTakesCorrectly()
        {
            _context.seekers.RemoveRange(_context.seekers);
            _context.users.RemoveRange(_context.users);
            _context.SaveChanges();
            // Arrange
            var users = new List<User>
            {
                new User { Username = "a@mail.com" },
                new User { Username = "b@mail.com" },
                new User { Username = "c@mail.com" }
            };
            await _context.users.AddRangeAsync(users);

            var seekers = new List<Seeker>
            {
                new Seeker { FirstName = "Anna", UserId = users[0].guid, user = users[0], IsDeleted = false },
                new Seeker { FirstName = "Brian", UserId = users[1].guid, user = users[1], IsDeleted = false },
                new Seeker { FirstName = "Cathy", UserId = users[2].guid, user = users[2], IsDeleted = false }
            };
            await _context.seekers.AddRangeAsync(seekers);

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetPaged(2, 1);

            // Assert
            Assert.Single(result);
            Assert.Equal("Brian", result.First().FirstName);
        }

        [Fact]
        public async Task GetPaged_DoesNotReturnDeletedSeekers()
        {
            _context.seekers.RemoveRange(_context.seekers);
            _context.users.RemoveRange(_context.users);
            _context.SaveChanges();
            // Arrange
            var user = new User { Username = "deleted@mail.com" };
            await _context.users.AddAsync(user);

            var seeker = new Seeker { FirstName = "Deleted", UserId = user.guid, user = user, IsDeleted = true };
            await _context.seekers.AddAsync(seeker);

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetPaged(1, 10);

            // Assert
            Assert.DoesNotContain(result, s => s.IsDeleted);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}