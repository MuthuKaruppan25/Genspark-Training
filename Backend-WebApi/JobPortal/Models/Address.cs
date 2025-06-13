
namespace JobPortal.Models;

public class Address
{
    public Guid guid { get; set; } = Guid.NewGuid();

    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public string AddressType { get; set; } = string.Empty;
    public Guid companyId { get; set; }
    public Company? company { get; set; }

}