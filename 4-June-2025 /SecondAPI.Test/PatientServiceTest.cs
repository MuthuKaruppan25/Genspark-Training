
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using SecondWebApi.Contexts;
using SecondWebApi.Interfaces;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

public class PatientServiceTest
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
    public async Task AddPatient()
    {
        //Arrange
        var userRepositoryMock = new Mock<IRepository<string, User>>();
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var mapperMock = new Mock<IMapper>();
        var PatientRepositoryMock = new Mock<IRepository<int, Patient>>();

        var patientAddDto = new PatientAddDto
        {
            Name = "Muthu",
            Age = 21,
            Email = "muthukaruppan@gmail.com",
            Password = "bdeb09230",
            Phone = "9000292022"
        };

        var encrypted = new EncryptModel
        {
            EncryptedData = Encoding.UTF8.GetBytes(patientAddDto.Password),
            HashKey = Encoding.UTF8.GetBytes("dwubiuwqgd78qg3")
        };

        var user = new User
        {
            username = patientAddDto.Email,
            password = encrypted.EncryptedData,
            HashKey = encrypted.HashKey,
            role = "Patient"
        };

        var patient = new Patient
        {
            Id = 1,
            Name = patientAddDto.Name,
            Age = patientAddDto.Age,
            Email = patientAddDto.Email,
            Phone = patientAddDto.Phone,

        };

        mapperMock.Setup(m => m.Map<PatientAddDto, User>(patientAddDto)).Returns(user);
        encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<EncryptModel>())).ReturnsAsync(encrypted);
        userRepositoryMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(user);
        PatientRepositoryMock.Setup(p => p.Add(It.IsAny<Patient>())).ReturnsAsync(patient);

        IPatientService patientService = new PatientService(
            PatientRepositoryMock.Object,
            mapperMock.Object,
            userRepositoryMock.Object,
            encryptionServiceMock.Object
        );

        //Action
        var result = await patientService.AddPatientAsync(patientAddDto);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Muthu"));


    }

    [Test]
    public async Task DeletePatient_CallsRepositoryDelete()
    {
        // Arrange
        var userRepositoryMock = new Mock<IRepository<string, User>>();
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var mapperMock = new Mock<IMapper>();
        var patientRepositoryMock = new Mock<IRepository<int, Patient>>();

        var patientService = new PatientService(
            patientRepositoryMock.Object,
            mapperMock.Object,
            userRepositoryMock.Object,
            encryptionServiceMock.Object
        );

        var patientId = 1;

        patientRepositoryMock.Setup(r => r.Delete(patientId)).ReturnsAsync(new Patient { Id = patientId });

        // Act
        await patientService.DeletePatient(patientId);

        // Assert
        patientRepositoryMock.Verify(r => r.Delete(patientId), Times.Once);
    }

    [Test]
    public async Task UpdatePatient_SuccessfulUpdate()
    {
        // Arrange
        var patientRepositoryMock = new Mock<IRepository<int, Patient>>();
        var userRepositoryMock = new Mock<IRepository<string, User>>();
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var mapperMock = new Mock<IMapper>();

        var patient = new Patient
        {
            Id = 1,
            Name = "Updated Name",
            Age = 22,
            Email = "updated@email.com",
            Phone = "1234567890"
        };

        patientRepositoryMock.Setup(r => r.Update(patient.Id, patient)).ReturnsAsync(patient);

        var patientService = new PatientService(
            patientRepositoryMock.Object,
            mapperMock.Object,
            userRepositoryMock.Object,
            encryptionServiceMock.Object
        );

        // Act
        var result = await patientService.UpdatePatient(patient.Id, patient);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Updated Name"));
    }

    [Test]
    public async Task GetPatientByName_ReturnsMatchingPatients()
    {
        // Arrange
        var patientRepositoryMock = new Mock<IRepository<int, Patient>>();
        var userRepositoryMock = new Mock<IRepository<string, User>>();
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var mapperMock = new Mock<IMapper>();

        var nameToSearch = "John";

        var patients = new List<Patient>
    {
        new Patient { Id = 1, Name = "John", Email = "john@example.com" },
        new Patient { Id = 2, Name = "Alice", Email = "alice@example.com" },
        new Patient { Id = 3, Name = "john", Email = "john2@example.com" }
    };

        patientRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(patients);

        var patientService = new PatientService(
            patientRepositoryMock.Object,
            mapperMock.Object,
            userRepositoryMock.Object,
            encryptionServiceMock.Object
        );

        // Act
        var result = await patientService.GetPatientByName(nameToSearch);

        // Assert
        Assert.That(result!.Count(), Is.EqualTo(2));
    }





    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}