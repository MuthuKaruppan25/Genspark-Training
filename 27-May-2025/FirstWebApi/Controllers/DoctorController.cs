
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private readonly DoctorService _doctorService;

    public DoctorController(DoctorService doctorService)
    {
        _doctorService = doctorService ?? throw new ArgumentNullException(nameof(doctorService), "Doctor service cannot be null");
    }

    [HttpGet]
    public ActionResult<IEnumerable<Doctor>> GetAllDoctors()
    {
        var doctors = _doctorService.GetAllDoctors();
        if (doctors == null || !doctors.Any())
        {
            return NotFound("No doctors found.");
        }
        return Ok(doctors);
    }
    [HttpGet("{id}")]
    public ActionResult<Doctor> GetDoctorById(int id)
    {
        var doctor = _doctorService.GetDoctorById(id);
        if (doctor == null)
        {
            return NotFound($"Doctor with ID {id} not found.");
        }
        return Ok(doctor);
    }
    [HttpPost]
    public ActionResult<Doctor> AddDoctor([FromBody] Doctor doctor)
    {
        if (doctor == null)
        {
            return BadRequest("Doctor cannot be null.");
        }
        var addedDoctor = _doctorService.AddDoctor(doctor);
        if (addedDoctor == null)
        {
            return BadRequest("An error occurred while adding the doctor.");
        }
        return CreatedAtAction(nameof(GetDoctorById), new { id = addedDoctor.Id }, addedDoctor);
    }
    [HttpDelete("{id}")]
    public IActionResult DeleteDoctor(int id)
    {


        _doctorService.DeleteDoctor(id);
        return NoContent();

    }
    [HttpPut("{id}")]
    public ActionResult<Doctor> UpdateDoctor(int id, [FromBody] Doctor doctor)
    {
        if (doctor == null)
        {
            return BadRequest("Doctor cannot be null.");
        }
        var updatedDoctor = _doctorService.UpdateDoctor(id, doctor);
        if (updatedDoctor == null)
        {
            return NotFound($"Doctor with ID {id} not found.");
        }
        return Ok(updatedDoctor);
    }
}