using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly PatientService _patientService;

    public PatientController(PatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public IActionResult GetAllPatients()
    {
        var patients = _patientService.GetAllPatients();
        return Ok(patients);
    }

    [HttpGet("{id}")]
    public IActionResult GetPatientById(int id)
    {
        var patient = _patientService.GetPatientById(id);
        if (patient == null) return NotFound();
        return Ok(patient);
    }

    [HttpPost]
    public IActionResult AddPatient([FromBody] Patient patient)
    {
        var result = _patientService.AddPatient(patient);
        if (result == null) return BadRequest();
        return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient);
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePatient(int id)
    {
        _patientService.DeletePatient(id);
        return NoContent();
    }
    [HttpPut("{id}")]
    public IActionResult UpdatePatient(int id, [FromBody] Patient patient)
    {
        var updatedPatient = _patientService.UpdatePatient(id, patient);
        if (updatedPatient == null) return NotFound();
        return Ok(updatedPatient);
    }
}