
using System.ComponentModel.DataAnnotations;

public class PasswordValidation : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
            return false;

        var password = value.ToString();
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // Password must be at least 8 characters long, contain at least one uppercase letter,
        // one lowercase letter, one digit, and one special character.
        var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
        return System.Text.RegularExpressions.Regex.IsMatch(password, pattern);
    }
}