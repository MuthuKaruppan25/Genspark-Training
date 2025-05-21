using System;
namespace HospitalManagement.Exceptions;

public class DuplicateEntityException : Exception
{
    private string _message = "Duplicate Entity Found";

    public DuplicateEntityException(string msg)
    {
        _message = msg;
    }
    public override string Message => _message;
}