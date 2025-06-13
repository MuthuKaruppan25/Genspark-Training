using Xunit;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JobPortal.Contexts;
using JobPortal.Models;
using JobPortal.Repositories;

namespace JobPortal.Tests
{
    public class FileRepositoryTests : IDisposable
    {
        private readonly JobContext _context;
        private readonly FileRepository _repository;

        public FileRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<JobContext>()
                .UseInMemoryDatabase("FileTestDb")
                .Options;

            _context = new JobContext(options);
            _repository = new FileRepository(_context);
        }

        [Fact]
        public async Task AddFile_ShouldSaveSuccessfully()
        {
            var file = new FileModel
            {
                Name = "resume.pdf",
                Data = new byte[] { 1, 2, 3 },
                Type = "application/pdf",
                Size = 3,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _repository.Add(file);

            var result = await _repository.Get(file.guid);

            Assert.NotNull(result);
            Assert.Equal("resume.pdf", result.Name);
            Assert.Equal("application/pdf", result.Type);
            Assert.Equal(3, result.Size);
        }

        [Fact]
        public async Task Get_InvalidFileId_ShouldThrowException()
        {
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _repository.Get(Guid.NewGuid()));

            Assert.Equal("No such user found", ex.Message);
        }

        [Fact]
        public async Task GetAllFiles_ShouldReturnFiles()
        {
            var file = new FileModel
            {
                Name = "coverletter.pdf",
                Data = new byte[] { 4, 5, 6 },
                Type = "application/pdf",
                Size = 3,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _repository.Add(file);

            var result = await _repository.GetAll();

            Assert.NotEmpty(result);
          
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}