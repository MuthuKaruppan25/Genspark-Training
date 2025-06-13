

using System.ComponentModel.DataAnnotations;

public class PhoneValidation : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
            return false;

        var phone = value.ToString();
        if (string.IsNullOrWhiteSpace(phone))
            return false;
        
        if (phone.Length < 10 || phone.Length > 15)
            return false;

    
        var pattern = @"^\+?[1-9]\d{1,14}$";
        return System.Text.RegularExpressions.Regex.IsMatch(phone, pattern);
    }
}