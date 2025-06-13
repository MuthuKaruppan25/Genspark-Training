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
    public class CompanyRepositoryTests : IDisposable
    {
        private readonly JobContext _context;
        private readonly CompanyRepository _repository;

        public CompanyRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<JobContext>()
                .UseInMemoryDatabase("CompanyTestDb")
                .Options;

            _context = new JobContext(options);
            _repository = new CompanyRepository(_context);
        }

        [Fact]
        public async Task AddCompany_ShouldSaveSuccessfully()
        {
            var industry = new IndustryType { Name = "IT" };
            await _context.industryTypes.AddAsync(industry);
            await _context.SaveChangesAsync();

            var company = new Company
            {
                CompanyName = "TestCompany",
                Description = "A test company",
                WebsiteUrl = "https://test.com",
                IndustryTypeId = industry.guid
            };

            await _repository.Add(company);

            var result = await _repository.Get(company.guid);

            Assert.NotNull(result);
            Assert.Equal("TestCompany", result.CompanyName);
            Assert.Equal(industry.guid, result.IndustryTypeId);
        }

        [Fact]
        public async Task Get_InvalidCompanyId_ShouldThrow_RecordNotFoundException()
        {
            var ex = await Assert.ThrowsAsync<RecordNotFoundException>(() =>
                _repository.Get(Guid.NewGuid()));

            Assert.Equal("User with the given Id Not Found", ex.Message);
        }

        [Fact]
        public async Task GetAllCompanies_ShouldReturnCompanies()
        {
            var industry = new IndustryType { Name = "AllIndustry" };
            await _context.industryTypes.AddAsync(industry);
            await _context.SaveChangesAsync();

            var company = new Company
            {
                CompanyName = "AllCo",
                Description = "All company",
                WebsiteUrl = "https://allco.com",
                IndustryTypeId = industry.guid
            };
            await _repository.Add(company);

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