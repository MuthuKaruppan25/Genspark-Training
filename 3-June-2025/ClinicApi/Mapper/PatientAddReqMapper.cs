

using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

public class PatientAddReqMapper
{
    public Patient? MapPatientAddRequest(PatientAddDto patientAddDto)
    {
        Patient patient = new();
        patient.Name = patientAddDto.Name;
        patient.Age = patientAddDto.Age;
        patient.Email = patientAddDto.Email;
        patient.Phone = patientAddDto.Phone;
        return patient;
    }
}