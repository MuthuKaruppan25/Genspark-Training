using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class EmailValidation : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
            return false;

        var email = value.ToString();
        if (string.IsNullOrWhiteSpace(email))
            return false;

        
        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }
}