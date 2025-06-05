

using DocumentShare.Models;

public class UserMapper
{
    public User? MapAddUser(UserRegisterDto userRegisterDto, EncryptModel encryptModel)
    {
        User user = new();
        user.Username = userRegisterDto.Username;
        user.PasswordHash = encryptModel.EncryptedData ?? Array.Empty<byte>();
        user.HashKey = encryptModel.HashKey ?? Array.Empty<byte>();
        user.Role = userRegisterDto.Role;
        user.Name = userRegisterDto.Name;
        user.Department = userRegisterDto.Department;
        return user;
    }
}