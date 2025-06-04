using System.Security.Claims;
using Moq;
using SecondWebApi.Interfaces;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;



public class AppointmentServiceTests
{
    private Mock<IRepository<string, Appointment>> _appointmentRepositoryMock = null!;
    private Mock<IRepository<int, Doctor>> _doctorRepositoryMock = null!;
    private Mock<IRepository<int, Patient>> _patientRepositoryMock = null!;
    private AppointmentService _appointmentService = null!;

    [SetUp]
    public void Setup()
    {
        _appointmentRepositoryMock = new Mock<IRepository<string, Appointment>>();
        _doctorRepositoryMock = new Mock<IRepository<int, Doctor>>();
        _patientRepositoryMock = new Mock<IRepository<int, Patient>>();

        _appointmentService = new AppointmentService(
            _appointmentRepositoryMock.Object,
            _doctorRepositoryMock.Object,
            _patientRepositoryMock.Object);
    }

    [Test]
    public async Task CreateAppointment_ShouldReturnAppointment_WhenDoctorAndPatientExist()
    {
        // Arrange
        var dto = new AppointmentAddDto
        {

            DoctorId = 1,
            PatientId = 2,
            AppointmnetDateTime = DateTime.Now,
            
        };

        _doctorRepositoryMock.Setup(r => r.Get(1)).ReturnsAsync(new Doctor { Id = 1 });
        _patientRepositoryMock.Setup(r => r.Get(2)).ReturnsAsync(new Patient { Id = 2 });
        _appointmentRepositoryMock.Setup(r => r.Add(It.IsAny<Appointment>()))
            .ReturnsAsync((Appointment a) => a);

        // Act
        var result = await _appointmentService.CreateAppointment(dto);

        // Assert
        Assert.That(result, Is.Not.Null);

    }
    [Test]
    public async Task CancelAppointment_ShouldSucceed_WhenValidDoctorAndAppointment()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "muthu@email.com") };
        var identity = new ClaimsIdentity(claims, "role");
        var principal = new ClaimsPrincipal(identity);

        var doctor = new Doctor { Id = 1, Email = "muthu@email.com" };
        var appointment = new Appointment { AppointmnetNumber="A1", DoctorId = 1, PatientId= 1,Status = "Scheduled" };

        _doctorRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Doctor> { doctor });
        _appointmentRepositoryMock.Setup(r => r.Get("A1")).ReturnsAsync(appointment);
        _appointmentRepositoryMock.Setup(r => r.Update("A1", It.IsAny<Appointment>()))
            .ReturnsAsync((string _, Appointment a) => a);

        // Act
        var result = await _appointmentService.CancelAppointment("A1", principal);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Status, Is.EqualTo("Cancelled"));
    }



}