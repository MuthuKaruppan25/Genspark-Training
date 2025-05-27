using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly AppointmentService _appointmentService;

    public AppointmentController(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public IActionResult GetAllAppointments()
    {
        var appointments = _appointmentService.GetAllAppointments();
        return Ok(appointments);
    }

    [HttpGet("{id}")]
    public IActionResult GetAppointmentById(int id)
    {
        var appointment = _appointmentService.GetAppointmentById(id);
        if (appointment == null) return NotFound();
        return Ok(appointment);
    }

    [HttpPost]
    public IActionResult CreateAppointment([FromBody] Appointment appointment)
    {
        var result = _appointmentService.CreateAppointment(appointment);
        if (result == null) return BadRequest();
        return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
    }

    [HttpDelete("{id}")]
    public IActionResult CancelAppointment(int id)
    {
        _appointmentService.CancelAppointment(id);
        return NoContent();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateAppointment(int id, [FromBody] Appointment appointment)
    {
        var updatedAppointment = _appointmentService.RescheduleAppointment(id, appointment);
        if (updatedAppointment == null) return NotFound();
        return Ok(updatedAppointment);
    }
}