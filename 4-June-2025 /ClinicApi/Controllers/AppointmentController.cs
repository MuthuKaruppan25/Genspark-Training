using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using SecondWebApi.Interfaces;

namespace SecondWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

       
        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateAppointment([FromBody] AppointmentAddDto appointmentAddDto)
        {
            var result = await _appointmentService.CreateAppointment(appointmentAddDto);
            if (result == null)
            {
                return BadRequest("Doctor or Patient not found.");
            }
            return CreatedAtAction(nameof(GetAppointment), new { id = result.AppointmnetNumber }, result);
        }


        [HttpPut("cancel/{id}")]
        [Authorize(Policy = "DoctorWithExperience5")]
        public async Task<ActionResult<Appointment>> CancelAppointment(string id)
        {
            var result = await _appointmentService.CancelAppointment(id,User);
            if (result == null)
            {
                return NotFound($"No appointment found with ID {id}");
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(string id)
        {
            var appointment = await _appointmentService.GetAppointment(id);
            if (appointment == null)
            {
                return NotFound($"No appointment found with ID {id}");
            }
            return Ok(appointment);
        }
    }
}
