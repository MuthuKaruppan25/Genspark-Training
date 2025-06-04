using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using SecondWebApi.Contexts;
using SecondWebApi.Interfaces;
using SecondWebApi.Misc;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;
using SecondWebApi.Repositories;
namespace SecondAPI.Test;

public class DoctorServiceTest
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
    [TestCase("General")]
    public async Task TestGetSpeciality(string speciality)
    {
        Mock<DoctorRepository> dockerRepositoryMock = new Mock<DoctorRepository>(_context);
        Mock<SpecialityRepository> specialityRepositoryMock = new(_context);
        Mock<DoctorSpecialityRepository> doctorSpecialityRepositoryMock = new(_context);
        Mock<UserRepository> userRepositoryMock = new(_context);
        Mock<OtherFuncinalitiesImplementation> otherContextFunctionitiesMock = new(_context);
        Mock<EncryptionService> encryptionServiceMock = new();
        Mock<IMapper> mapperMock = new();

        otherContextFunctionitiesMock.Setup(ocf => ocf.GetDoctorsBySpeciality(It.IsAny<string>()))
                            .ReturnsAsync((string specilaity) => new List<DoctorsBySpecialityResponseDto>{
                                   new DoctorsBySpecialityResponseDto
                                        {
                                            Dname = "test",
                                            Yoe = 2,
                                            Id=1
                                        }
                    });
        IDoctorService doctorService = new DoctorService(dockerRepositoryMock.Object,
                                                specialityRepositoryMock.Object,
                                                doctorSpecialityRepositoryMock.Object,
                                                encryptionServiceMock.Object,
                                                mapperMock.Object,
                                                userRepositoryMock.Object,
                                                otherContextFunctionitiesMock.Object
                                                );


        //Assert.That(doctorService, Is.Not.Null);
        //Action
        var result = await doctorService.GetDoctorsBySpeciality(speciality);
        //Assert
        Assert.That(result.Count(), Is.EqualTo(1));

    }

    [Test]
    public async Task AddDoctor_Success()
    {
        // Arrange
        var doctorAddDto = new DoctorAddDto
        {
            Name = "Dr. Test",
            Email = "dr.test@example.com",
            Password = "Secure123",
            YearsOfExperience = 5,
            specialities = new List<SpecialityAddDto>
        {
            new SpecialityAddDto { Name = "Cardiology" }
        }
        };

        var encrypted = new EncryptModel
        {
            EncryptedData = Encoding.UTF8.GetBytes(doctorAddDto.Password),
            HashKey = Encoding.UTF8.GetBytes("Key")
        };

        var user = new User
        {
            username = doctorAddDto.Email,
            password = encrypted.EncryptedData,
            HashKey = encrypted.HashKey,
            role = "Doctor"
        };

        var doctor = new Doctor
        {
            Id = 1,
            Name = doctorAddDto.Name,
            Email = doctorAddDto.Email,
            YearsOfExperience = doctorAddDto.YearsOfExperience,
        };

        var speciality = new Speciality
        {
            Id = 101,
            Name = "Cardiology"
        };

        var doctorSpeciality = new DoctorSpeciality
        {
            DoctorId = 1,
            SpecialityId = 101
        };


        var doctorRepositoryMock = new Mock<IRepository<int, Doctor>>();
        var specialityRepositoryMock = new Mock<IRepository<int, Speciality>>();
        var doctorSpecialityRepositoryMock = new Mock<IRepository<int, DoctorSpeciality>>();
        var userRepositoryMock = new Mock<IRepository<string, User>>();
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var mapperMock = new Mock<IMapper>();
        var otherContextFunctionitiesMock = new Mock<IOtherContextFunctionities>();


        mapperMock.Setup(m => m.Map<DoctorAddDto, User>(doctorAddDto)).Returns(user);
        encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<EncryptModel>())).ReturnsAsync(encrypted);
        userRepositoryMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(user);
        doctorRepositoryMock.Setup(r => r.Add(It.IsAny<Doctor>())).ReturnsAsync(doctor);

        specialityRepositoryMock.Setup(s => s.GetAll()).ReturnsAsync(new List<Speciality>());
        specialityRepositoryMock.Setup(s => s.Add(It.IsAny<Speciality>())).ReturnsAsync(speciality);

        doctorSpecialityRepositoryMock.Setup(ds => ds.Add(It.IsAny<DoctorSpeciality>())).ReturnsAsync(doctorSpeciality);


        IDoctorService doctorService = new DoctorService(
            doctorRepositoryMock.Object,
            specialityRepositoryMock.Object,
            doctorSpecialityRepositoryMock.Object,
            encryptionServiceMock.Object,
            mapperMock.Object,
            userRepositoryMock.Object,
            otherContextFunctionitiesMock.Object
        );

        // Act
        var result = await doctorService.AddDoctor(doctorAddDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Dr. Test"));
        Assert.That(result.Email, Is.EqualTo("dr.test@example.com"));
    }

    [Test]
    public async Task DoctorGetByName_Success()
    {
        //Arrange
        var doctorRepositoryMock = new Mock<IRepository<int, Doctor>>();
        var specialityRepositoryMock = new Mock<IRepository<int, Speciality>>();
        var doctorSpecialityRepositoryMock = new Mock<IRepository<int, DoctorSpeciality>>();
        var userRepositoryMock = new Mock<IRepository<string, User>>();
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var mapperMock = new Mock<IMapper>();
        var otherContextFunctionitiesMock = new Mock<IOtherContextFunctionities>();

        var doctorList = new List<Doctor>
                            {
                                new Doctor { Id = 1, Name = "Alice", Email = "alice@example.com",YearsOfExperience = 5},
                                new Doctor { Id = 2, Name = "Dr. Bob", Email = "bob@example.com",YearsOfExperience=8 }
                            };
        doctorRepositoryMock.Setup(ds => ds.GetAll()).ReturnsAsync(doctorList);


        IDoctorService doctorService = new DoctorService(
            doctorRepositoryMock.Object,
            specialityRepositoryMock.Object,
            doctorSpecialityRepositoryMock.Object,
            encryptionServiceMock.Object,
            mapperMock.Object,
            userRepositoryMock.Object,
            otherContextFunctionitiesMock.Object
        );

        //Action
        var result = await doctorService.GetDoctorByName("Alice");

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Alice"));



    }



    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

}