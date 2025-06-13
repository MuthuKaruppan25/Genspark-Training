using JobPortal.Models;
using JobPortal.Models.DTOs;

public class UserMapper
{
    public User? MapUser(string email, EncryptModel encryptModel,string role)
    {
        return new User
        {
            guid = Guid.NewGuid(),
            Username = email,
            PasswordHash = encryptModel.EncryptedData!,
            HashKey = encryptModel.HashKey!,
            RefreshToken = string.Empty,
            Role = role,
            RefreshTokenExpiryTime = DateTime.MinValue,
            IsDeleted = false
        };
    }
}
