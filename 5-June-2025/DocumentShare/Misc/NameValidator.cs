
using System.ComponentModel.DataAnnotations;

namespace DocumentShare.Misc;
public class TextValidator : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is string text)
        {
            return IsValidName(text);
        }
        return false;
    }
    public static bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }


        foreach (char c in name)
        {
            if (!char.IsLetter(c) && c != ' ' && c != '-')
            {
                return false;
            }
        }

        if (name.Length < 2 || name.Length > 50)
        {
            return false;
        }

        return true;
    }
}