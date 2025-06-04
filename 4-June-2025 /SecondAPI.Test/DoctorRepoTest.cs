using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SecondWebApi.Contexts;
using SecondWebApi.Interfaces;
using SecondWebApi.Models;
using SecondWebApi.Repositories;

namespace SecondAPI.Test
{
    public class DoctorRepositoryTests
    {
        private ClinicContext _context;
        private ClinicContext _contextEmpty;
        private IRepository<int, Doctor> _repository;
        private IRepository<int, Doctor> _emptyRepository;

        [SetUp]
        public void Setup()
        {
            var options1 = new DbContextOptionsBuilder<ClinicContext>()
                                .UseInMemoryDatabase("DoctorTestDb")
                                .Options;

            var options2 = new DbContextOptionsBuilder<ClinicContext>()
                                .UseInMemoryDatabase("EmptyDoctorTestDb")
                                .Options;

            _context = new ClinicContext(options1);
            _contextEmpty = new ClinicContext(options2);

            _repository = new DoctorRepository(_context);
            _emptyRepository = new DoctorRepository(_contextEmpty);
        }

        [Test]
        public async Task AddDoctor_Success()
        {
            // Arrange
            var doctor = new Doctor
            {
                Name = "Dr. Smith",
                Email = "smith@example.com",
                Status = "Active",
                YearsOfExperience = 10
            };

            // Act
            await _repository.Add(doctor);
            await _context.SaveChangesAsync();

            var result = await _repository.Get(doctor.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Dr. Smith"));

        }

        [Test]
        public async Task GetDoctorById_Exception()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.Get(999));
            Assert.That(ex.Message, Is.EqualTo("No doctor with the given ID"));
        }

        [Test]
        public async Task GetAllDoctors_Success()
        {
            // Arrange
            var doctor = new Doctor
            {
                Name = "Dr. Smith",
                Email = "smith@example.com",
                Status = "Active",
                YearsOfExperience = 10
            };

            await _repository.Add(doctor);
            await _context.SaveChangesAsync();

            // Act
            var doctors = await _repository.GetAll();

            // Assert
            Assert.That(doctors.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllDoctors_Exception()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _emptyRepository.GetAll());
            Assert.That(ex.Message, Is.EqualTo("No Doctors Found"));
        }

        [Test]
        public async Task UpdateDoctor_Success()
        {
            // Arrange
            var doctor = new Doctor
            {
                Name = "Dr. Smith",
                Email = "smith@example.com",
                Status = "Active",
                YearsOfExperience = 10
            };

            await _repository.Add(doctor);
            await _context.SaveChangesAsync();

            // Act
            doctor.Name = "Dr. Smith";
            var updatedDoctor = await _repository.Update(doctor.Id, doctor);
            await _context.SaveChangesAsync();

            // Assert
            Assert.That(updatedDoctor.Name, Is.EqualTo("Dr. Smith"));
        }

        [Test]
        public async Task UpdateDoctor_Exception()
        {
            // Arrange
            var nonExistentDoctor = new Doctor
            {
                Name = "Dr. Smith",
                Email = "smith@example.com",
                Status = "Active",
                YearsOfExperience = 10
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.Update(nonExistentDoctor.Id, nonExistentDoctor));
            Assert.That(ex.Message, Is.EqualTo("No doctor with the given ID"));
        }

        [Test]
        public async Task DeleteDoctor_Success()
        {
            // Arrange
            var doctor = new Doctor
            {
                Name = "Dr. Smith",
                Email = "smith@example.com",
                Status = "Active",
                YearsOfExperience = 10
            };

            await _repository.Add(doctor);
            await _context.SaveChangesAsync();

            // Act
            var deletedDoctor = await _repository.Delete(doctor.Id);
            await _context.SaveChangesAsync();

            // Assert
            Assert.That(deletedDoctor.Id, Is.EqualTo(doctor.Id));
        }

        [Test]
        public async Task DeleteDoctor_Exception()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.Delete(999));
            Assert.That(ex.Message, Is.EqualTo("No doctor with the given ID"));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            _contextEmpty.Dispose();
        }
    }
}
