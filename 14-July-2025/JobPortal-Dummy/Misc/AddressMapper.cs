using JobPortal.Models;
using JobPortal.Models.DTOs;

public class AddressMapper
{
    public List<Address> MapAddresses(List<AddressRegisterDto>? addressDtos, Guid companyId)
    {
        if (addressDtos == null || !addressDtos.Any())
            return new List<Address>();

        return addressDtos.Select(addr => new Address
        {
            guid = Guid.NewGuid(),
            AddressLine1 = addr.AddressLine1,
            AddressLine2 = addr.AddressLine2,
            City = addr.City,
            State = addr.State,
            PostalCode = addr.PostalCode,
            Country = addr.Country,
            AddressType = addr.AddressType,
            companyId = companyId
        }).ToList();
    }
}
