using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    static List<Patient> patients = new List<Patient>
    {
        new Patient { Id = 201, Name = "John", Age = 30, Reason = "Fever" },
        new Patient { Id = 202, Name = "Alice", Age = 25, Reason ="Cold" },
    };

    [HttpGet]
    public IActionResult GetPatients()
    {
        return Ok(patients);
    }

    [HttpGet("{id}")]
    public IActionResult GetPatient(int id)
    {
        var patient = patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            return NotFound();
        }
        return Ok(patient);
    }

    [HttpPost]
    public IActionResult AddPatient(Patient patient)
    {
        patients.Add(patient);
        return Created("", patient);
    }

    [HttpPut("{id}")]
    public IActionResult UpdatePatient(int id, Patient patient)
    {
        if (id != patient.Id)
        {
            return BadRequest("Id in URL and patient.Id do not match.");
        }

        var existingPatientIndex = patients.FindIndex(p => p.Id == id);
        if (existingPatientIndex == -1)
        {
            return NotFound();
        }

        patients[existingPatientIndex] = patient;
        return Ok(patient);
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePatient(int id)
    {
        var existingPatientIndex = patients.FindIndex(p => p.Id == id);
        if (existingPatientIndex == -1)
        {
            return NotFound();
        }

        patients.RemoveAt(existingPatientIndex);
        return NoContent();
    }
}
