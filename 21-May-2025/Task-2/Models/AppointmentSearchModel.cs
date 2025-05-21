using System;

namespace HospitalManagement.Models;

public class AppointmentSearchModel
{
    public int? AppointmentId{get;set;}
    public string? PatientName {get;set;}
    
    public DateTime? AppointmentDate {get;set;}

    public Range<int>? Age {get; set;}
}
public class Range<T>
{
    public T? MinVal {get; set;}
    public T? MaxVal {get; set;}
}