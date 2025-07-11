
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

   
        var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#_])[A-Za-z\d@$!%*?&]{8,}$";
        return System.Text.RegularExpressions.Regex.IsMatch(password, pattern);
    }
}