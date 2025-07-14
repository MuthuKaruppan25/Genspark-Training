using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class PostalCodeValidator : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is string postalCode)
        {
            return IsValidPostalCode(postalCode);
        }
        return false;
    }

    public static bool IsValidPostalCode(string postalCode)
    {
        
        return Regex.IsMatch(postalCode, @"^\d{6}$");
    }
}