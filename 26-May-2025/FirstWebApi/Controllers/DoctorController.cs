using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class DoctorController : ControllerBase
{
    static List<Doctor> doctors = new List<Doctor>
    {
        new Doctor{Id=101,Name="Ramu"},
        new Doctor{Id=102,Name="Somu"},
    };
    [HttpGet]
    public IActionResult GetDoctors()
    {
        return Ok(doctors);
    }

    [HttpGet("{id}")]
    public IActionResult GetPatient(int id)
    {
        var doctor = doctors.FirstOrDefault(p => p.Id == id);
        if (doctor == null)
        {
            return NotFound();
        }
        return Ok(doctor);
    }
    [HttpPost]
    public IActionResult AddDoctor(Doctor doctor)
    {
        doctors.Add(doctor);
        return Created("", doctor);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateDoctor(int id,Doctor doctor)
    {
        if (id != doctor.Id)
        {
            return BadRequest("Id in URL and patient.Id do not match.");
        }
        var existingDoctorIndex = doctors.FindIndex(d => d.Id == doctor.Id);
        if (existingDoctorIndex == -1)
        {
            return NotFound();
        }
        doctors[existingDoctorIndex] = doctor;
        return Ok(doctor);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteDoctor(int id)
    {
        var existingDoctorIndex = doctors.FindIndex(d => d.Id == id);
        if (existingDoctorIndex == -1)
        {
            return NotFound();
        }
        doctors.RemoveAt(existingDoctorIndex);
        return NoContent();
    }
}