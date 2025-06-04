



using SecondWebApi.Models;

namespace SecondWebApi.Interfaces;

public interface IEncryptionService
{
    public Task<EncryptModel> EncryptData(EncryptModel data);
}