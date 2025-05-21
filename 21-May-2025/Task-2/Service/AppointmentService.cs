using System;
using System.Collections.Generic;
using System.Linq;
using HospitalManagement.Interfaces;
using HospitalManagement.Repositories;
using HospitalManagement.Models;

namespace HospitalManagement.Services
{
    public class AppointmentService : IAppointmentService
    {
        private IRepository<int, Appointment> _repository;

        public AppointmentService(IRepository<int, Appointment> repository)
        {
            _repository = repository;
        }

        public int AddAppointment(Appointment appointment)
        {
            try
            {
                var apt = _repository.Add(appointment);
                if (apt is not null)
                {
                    return apt.Id;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return -1;
        }

        public List<Appointment>? SearchAppointment(AppointmentSearchModel appointmentSearchModel)
        {
            try
            {
                var appointments = _repository.GetAll();

                appointments = SearchAppointmentByPatientName(appointments, appointmentSearchModel.PatientName);
                appointments = SearchAppointmentById(appointments, appointmentSearchModel.AppointmentId);
                appointments = SearchAppointmentByDate(appointments, appointmentSearchModel.AppointmentDate);
                appointments = SearchAppointmentByAge(appointments, appointmentSearchModel.Age);

                return appointments.ToList(); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public ICollection<Appointment> SearchAppointmentByPatientName(ICollection<Appointment> appointments, string? patientName)
        {
            if (string.IsNullOrWhiteSpace(patientName) || appointments.Count == 0)
                return appointments;

            return appointments.Where(e => 
                !string.IsNullOrEmpty(e.PatientName) && 
                e.PatientName.Equals(patientName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public ICollection<Appointment> SearchAppointmentById(ICollection<Appointment> appointments, int? id)
        {
            if (id is null || id == 0 || appointments.Count == 0)
                return appointments;

            return appointments.Where(e => e.Id == id).ToList();
        }

        public ICollection<Appointment> SearchAppointmentByDate(ICollection<Appointment> appointments, DateTime? date)
        {
            if (date is null || appointments.Count == 0)
                return appointments;

            return appointments.Where(e => e.AppointmentDate.Date == date.Value.Date).ToList();
        }

        public ICollection<Appointment> SearchAppointmentByAge(ICollection<Appointment> appointments, Range<int>? age)
        {
            if (age is null || appointments.Count == 0)
                return appointments;

            return appointments.Where(e => e.PatientAge >= age.MinVal && e.PatientAge <= age.MaxVal).ToList();
        }
    }
}
