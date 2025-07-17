








using JobPortal.Models;

namespace JobPortal.Interfaces;

public interface IEncryptionService
{
    public Task<EncryptModel> EncryptData(EncryptModel data);
}