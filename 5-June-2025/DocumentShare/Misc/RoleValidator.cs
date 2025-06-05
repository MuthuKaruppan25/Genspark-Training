

using System.ComponentModel.DataAnnotations;

public class RoleValidator : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is string role)
        {

            return role == "HRAdmin" || role == "Staff" || role == "Guest";
        }
        return false; 
    }   
}