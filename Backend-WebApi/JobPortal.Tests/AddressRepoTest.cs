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
    public class AddressRepositoryTests : IDisposable
    {
        private readonly JobContext _context;
        private readonly AddressRepository _repository;

        public AddressRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<JobContext>()
                .UseInMemoryDatabase("AddressTestDb")
                .Options;

            _context = new JobContext(options);
            _repository = new AddressRepository(_context);
        }

        [Fact]
        public async Task AddAddress_ShouldSaveSuccessfully()
        {
            var company = new Company { CompanyName = "TestCompany" };
            await _context.companies.AddAsync(company);
            await _context.SaveChangesAsync();

            var address = new Address
            {
                AddressLine1 = "123 Main St",
                AddressLine2 = "Apt 4B",
                City = "Metropolis",
                State = "NY",
                PostalCode = "12345",
                Country = "USA",
                AddressType = "Headquarters",
                companyId = company.guid
            };

            await _repository.Add(address);

            var result = await _repository.Get(address.guid);

            Assert.NotNull(result);
            Assert.Equal("123 Main St", result.AddressLine1);
            Assert.Equal(company.guid, result.companyId);
        }

        [Fact]
        public async Task Get_InvalidAddressId_ShouldThrow_RecordNotFoundException()
        {
            var ex = await Assert.ThrowsAsync<RecordNotFoundException>(() =>
                _repository.Get(Guid.NewGuid()));

            Assert.Equal("User with the given Id Not Found", ex.Message);
        }

        [Fact]
        public async Task GetAllAddresses_ShouldReturnAddresses()
        {
            var company = new Company { CompanyName = "AllCo" };
            await _context.companies.AddAsync(company);
            await _context.SaveChangesAsync();

            var address = new Address
            {
                AddressLine1 = "456 Elm St",
                City = "Gotham",
                State = "NJ",
                PostalCode = "54321",
                Country = "USA",
                AddressType = "Branch",
                companyId = company.guid
            };
            await _repository.Add(address);

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