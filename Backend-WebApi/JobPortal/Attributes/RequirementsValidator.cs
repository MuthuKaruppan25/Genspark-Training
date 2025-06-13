using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class RequirementsValidator : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is string text)
        {
            return IsValidExperience(text);
        }
        return false;
    }

    public static bool IsValidExperience(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

      
        var pattern = @"^[a-zA-Z0-9\s.,\-+()'/&:;]+$";
        if (!Regex.IsMatch(input, pattern))
            return false;

        
        if (input.Length < 2 || input.Length > 200)
            return false;

        return true;
    }
}