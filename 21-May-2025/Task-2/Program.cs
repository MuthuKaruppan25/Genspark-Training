using System;
using System.Collections.Generic;
using HospitalManagement.Models;
using HospitalManagement.Interfaces;
using HospitalManagement.Repositories;
using HospitalManagement.Services;

class Program
{
    static IAppointmentService appointmentService = new AppointmentService(new AppointmentRepository());

    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Hospital Management System");
        Console.WriteLine("----Services are Listed Below----");
        while (true)
        {
            Console.WriteLine("---Hospital Appointment Management---");
            Console.WriteLine("1. Add Appointment");
            Console.WriteLine("2. Search Appointment by Patient Name");
            Console.WriteLine("3. Search Appointment by Appointment Id");
            Console.WriteLine("4. Search Appointment by Appointment Date");
            Console.WriteLine("5. Search Appointment by Patient Age");
            Console.WriteLine("6. Search Appointment between the given range of Age");
            Console.WriteLine("7. Exit");
            Console.Write("Enter the choice: ");
            int choice = getUsersInput();
            Console.WriteLine();

            switch (choice)
            {
                case 1:
                    AddAppointment();
                    break;
                case 2:
                    SearchByPatientName();
                    break;
                case 3:
                    SearchByAppointmentId();
                    break;
                case 4:
                    SearchByAppointmentDate();
                    break;
                case 5:
                    SearchByPatientAge();
                    break;
                case 6:
                    SearchByAgeRange();
                    break;
                case 7:
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid Input! Enter the valid choice");
                    break;
            }
            Console.WriteLine();
        }
    }

    static void AddAppointment()
    {
        Console.WriteLine("Enter the Patient Name: ");
        string? name = GetStringInput();
        Console.WriteLine("Enter the Patient Age: ");
        int age = getUsersInput();
        DateTime appointmentDate = getDateTime();
        Console.WriteLine("Enter the Reason for Appointment: ");
        string? reason = GetStringInput();

        var appointment = new Appointment
        {

            PatientName = name!,
            PatientAge = age,
            AppointmentDate = appointmentDate,
            Reason = reason!
        };

        int result = appointmentService.AddAppointment(appointment);
        if (result != -1)
            Console.WriteLine($"Appointment Added Successfully with Id: {result}");
        else
            Console.WriteLine("Failed to Add Appointment");
    }

    static void SearchByPatientName()
    {
        Console.WriteLine("Enter the Patient Name to Search: ");
        string? name = GetStringInput();

        var results = appointmentService.SearchAppointment(new AppointmentSearchModel { PatientName = name });

        DisplayAppointments(results, $"No appointments found for Patient Name: {name}");
    }

    static void SearchByAppointmentId()
    {
        Console.WriteLine("Enter the Appointment Id to Search: ");
        int id = getUsersInput();

        var appointment = appointmentService.SearchAppointment(new AppointmentSearchModel { AppointmentId = id });
        if (appointment != null)
        {
            Console.WriteLine("Appointment Found:");
            DisplayAppointments(appointment, $"No appointment found with Id: {id}");
        }
        else
        {
            Console.WriteLine($"No appointment found with Id: {id}");
        }
    }

    static void SearchByAppointmentDate()
    {
        Console.WriteLine("Enter the Appointment Date to Search (DD/MM/YYYY): ");
        DateTime date = getDateTime();

        var results = appointmentService.SearchAppointment(new AppointmentSearchModel { AppointmentDate = date });

        DisplayAppointments(results, $"No appointments found on date: {date.ToShortDateString()}");
    }

    static void SearchByPatientAge()
    {
        Console.WriteLine("Enter the Patient Age to Search: ");
        int age = getUsersInput();

        var results = appointmentService.SearchAppointment(new AppointmentSearchModel { Age = new Range<int> { MinVal = age, MaxVal = age } });

        DisplayAppointments(results, $"No appointments found for Patient Age: {age}");
    }

    static void SearchByAgeRange()
    {
        Console.WriteLine("Enter the minimum age: ");
        int minAge = getUsersInput();
        Console.WriteLine("Enter the maximum age: ");
        int maxAge = getUsersInput();

        if (maxAge < minAge)
        {
            Console.WriteLine("Maximum age cannot be less than minimum age. Please try again.");
            return;
        }

        var results = appointmentService.SearchAppointment(new AppointmentSearchModel { Age = new Range<int> { MinVal = minAge, MaxVal = maxAge } });
        DisplayAppointments(results, $"No appointments found between ages {minAge} and {maxAge}");
    }



    static void DisplayAppointments(IEnumerable<Appointment> appointments, string emptyMessage)
    {
        bool foundAny = false;
        foreach (var appt in appointments)
        {
            DisplayAppointment(appt);
            foundAny = true;
        }
        if (!foundAny)
        {
            Console.WriteLine(emptyMessage);
        }
    }

    static void DisplayAppointment(Appointment appt)
    {
        Console.WriteLine($"Id: {appt.Id}, Patient: {appt.PatientName}, Age: {appt.PatientAge}, Date: {appt.AppointmentDate}, Reason: {appt.Reason}");
    }

    static DateTime getDateTime()
    {
        DateTime date;
        Console.Write("Enter the Appointment Date and Time (DD/MM/YYYY HH:mm): ");
        string? input = Console.ReadLine();

        while (true)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.Write("Input cannot be empty. Please enter a valid date and time (DD/MM/YYYY : HH:mm): ");
            }
            else if (DateTime.TryParse(input, out date))
            {
                return date;
            }
            else
            {
                Console.Write("Invalid input. Please enter a valid date and time (DD/MM/YYYY HH:mm): ");
            }
            input = Console.ReadLine();
        }
    }

    static string? GetStringInput()
    {
        string? name = Console.ReadLine()?.Trim();
        while (string.IsNullOrWhiteSpace(name) || !IsValidName(name))
        {
            Console.WriteLine("Invalid name. Please enter a valid name:");
            name = Console.ReadLine()?.Trim();
        }
        return name;
    }

    static bool IsValidName(string name)
    {
        foreach (char c in name)
        {
            if (!char.IsLetter(c) && c != ' ')
                return false;
        }
        return true;
    }

    static int getUsersInput()
    {
        int input;
        while (!int.TryParse(Console.ReadLine(), out input) || input < 0)
        {
            Console.Write("Invalid input. Please enter a valid number: ");
        }
        return input;
    }
}
