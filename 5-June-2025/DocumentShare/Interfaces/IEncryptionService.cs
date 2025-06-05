






using DocumentShare.Models;

namespace DocumentShare.Interfaces;

public interface IEncryptionService
{
    public Task<EncryptModel> EncryptData(EncryptModel data);
}