using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public class FileValidationAttribute : ValidationAttribute
{
    private readonly string[] _allowedExtensions = { ".pdf" };
    private readonly long _maxFileSizeInBytes = 10 * 1024 * 1024; // 2 MB

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IFormFile file)
            return new ValidationResult("File is Required");// If no file is uploaded, let other [Required] handle it

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!_allowedExtensions.Contains(extension))
        {
            return new ValidationResult("Only PDF files are allowed.");
        }

        if (file.Length > _maxFileSizeInBytes)
        {
            return new ValidationResult("File size should not exceed 2 MB.");
        }

        return ValidationResult.Success!;
    }
}
