using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class UrlValidator : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is string url)
        {
            return IsValidUrl(url);
        }
        return false;
    }

    public static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        // Basic URL pattern (http/https, optional www, domain, optional path/query)
        var pattern = @"^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?$";
        return Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase);
    }
}