using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models.DTOs;
public class AddressRegisterDto
{
    [RequirementsValidator]
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    [TextValidator]
    public string City { get; set; } = string.Empty;
    [TextValidator]
    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;
    [TextValidator]
    public string Country { get; set; } = string.Empty;
    [TextValidator]
    public string AddressType { get; set; } = string.Empty;
}