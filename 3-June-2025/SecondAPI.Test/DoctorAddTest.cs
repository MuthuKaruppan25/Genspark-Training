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
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ClinicContext>()
                            .UseInMemoryDatabase("TestDb")
                            .Options;
        _context = new ClinicContext(options);
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
        IRepository<int, Patient> repository = new PatientRepository(_context);
        await repository.Add(patient);
        await _context.SaveChangesAsync();
        var result = await repository.Get(patient.Id);


        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Muthu"));
    }

    [Test]
    public async Task GetById_Invalid()
    {
        // Arrange
        var patient = new Patient
        {
            Name = "Muthu",
            Age = 18,
            Email =  "muthu@gmail.com",
            Phone = "300220303201"
        };

        IRepository<int, Patient> repository = new PatientRepository(_context);
        await repository.Add(patient);
        await _context.SaveChangesAsync();

        // Action
        var result = await repository.Get(2);

        // Assert
        Assert.That(result, Is.Not.Null);

    }

    [Test]
    public async Task GetById_Exception()
    {
        // Arrange
        IRepository<int, Patient> repository = new PatientRepository(_context);

        // Action
        var ex = Assert.ThrowsAsync<Exception>(async () => await repository.Get(15));

        // Assert
        Assert.That(ex.Message, Is.EqualTo("No patient with the given ID"));
    }


    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}