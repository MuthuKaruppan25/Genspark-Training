using System;
using Microsoft.EntityFrameworkCore;
using SecondWebApi.Models;
using SecondWebApi.Contexts;
using SecondWebApi.Interfaces;
using SecondWebApi.Repositories;
using System.Threading.Tasks;
namespace SecondAPI.Test;

public class Tests
{
    private ClinicContext _context;

    private ClinicContext clincContext;
    private IRepository<int, Patient> repository;
    [SetUp]
    public void Setup()
    {
        var options1 = new DbContextOptionsBuilder<ClinicContext>()
                            .UseInMemoryDatabase("TestDb")
                            .Options;
        var options2 = new DbContextOptionsBuilder<ClinicContext>()
                            .UseInMemoryDatabase("TestDb Sample")
                            .Options;
        _context = new ClinicContext(options1);
        clincContext = new ClinicContext(options2);

        repository = new PatientRepository(_context);
    }

    [Test]
    public async Task Test1()
    {
        //Arrange
        var username = "muthu@gmail.com";
        var password = System.Text.Encoding.UTF8.GetBytes("Muthu@123");
        var HashKey = System.Text.Encoding.UTF8.GetBytes("UniqueKey");
        var role = "Patient";

        var user = new User
        {
            username = username,
            role = role,
            password = password,
            HashKey = HashKey
        };

        //Action
        _context.Add(user);
        _context.SaveChangesAsync();

        // Assert
        // var savedUser = _context.users.FirstOrDefault(u => u.username == "muthu@gmail.com");
        // Assert.That(savedUser, Is.Not.Null, "User not Saved");

        //Arrange
        var patient = new Patient
        {
            Name = "Muthu",
            Age = 18,
            Email = username,
            Phone = "300220303201"
        };

        //Action

        await repository.Add(patient);
        await _context.SaveChangesAsync();
        var result = await repository.Get(patient.Id);


        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Muthu"));
    }


    [Test]
    public async Task GetById_Exception()
    {

        // Action
        var ex = Assert.ThrowsAsync<Exception>(async () => await repository.Get(15));

        // Assert
        Assert.That(ex.Message, Is.EqualTo("No patient with the given ID"));
    }

    [Test]
    public async Task GetAllPatients()
    {
        //Arrange
        var patient = new Patient
        {
            Name = "Muthu",
            Age = 18,
            Email = "muthu@gmail.com",
            Phone = "300220303201"
        };


        await repository.Add(patient);
        await _context.SaveChangesAsync();

        // Action
        var result = await repository.GetAll();

        //Assert
        Assert.That(result.Count(), Is.EqualTo(1));

    }

    [Test]
    public async Task GetAllPatientsException()
    {
        //Arrange
        IRepository<int, Patient> _repository = new PatientRepository(clincContext);


        //Action
        var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetAll());

        // Assert
        Assert.That(ex.Message, Is.EqualTo("No Patients Found"));

    }

    [Test]
    public async Task UpdatePatient_Success()
    {
        // Arrange
        var patient = new Patient
        {
            Name = "Muthu",
            Age = 18,
            Email = "muthu@gmail.com",
            Phone = "300220303201"
        };

        await repository.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        patient.Name = "UpdatedMuthu";
        var updatedPatient = await repository.Update(patient.Id, patient);
        await _context.SaveChangesAsync();

        // Assert
        Assert.That(updatedPatient.Name, Is.EqualTo("UpdatedMuthu"));
    }

    [Test]
    public async Task UpdatePatient_Exception()
    {
        // Arrange
        var nonExistentPatient = new Patient
        {
            Id = 999,
            Name = "Ghost",
            Age = 30,
            Email = "ghost@example.com",
            Phone = "0000000000"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await repository.Update(nonExistentPatient.Id, nonExistentPatient));
        Assert.That(ex.Message, Is.EqualTo("No patient with the given ID"));
    }


    [Test]
    public async Task DeletePatient_Success()
    {
        // Arrange
        var patient = new Patient
        {
            Name = "Muthu",
            Age = 18,
            Email = "muthu@gmail.com",
            Phone = "300220303201"
        };

        await repository.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        var deletedPatient = await repository.Delete(patient.Id);
        await _context.SaveChangesAsync();

        // Assert
        Assert.That(deletedPatient.Id, Is.EqualTo(1));

    }

    [Test]
    
    public async Task DeletePatient_Exception()
    {
        // Action
        var ex = Assert.ThrowsAsync<Exception>(async () => await repository.Delete(999));

        //Assert
        Assert.That(ex.Message, Is.EqualTo("No patient with the given ID")); 
    }



    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
        clincContext.Dispose();
    }
}