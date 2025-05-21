using System;
namespace HospitalManagement.Exceptions;

public class CollectionEmptyException : Exception
{
    private string _message = "Collection is Empty";

    public CollectionEmptyException(string msg)
    {
        _message = msg;
    }
    public override string Message => _message;
}