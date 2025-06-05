


using DocumentShare.Interfaces;
using DocumentShare.Models;

public class UserService : IUserService
{
    UserMapper userMapper;
    private readonly IRepository<string, User> _repository;
    private readonly IEncryptionService _encryptionService;

    public UserService(IRepository<string, User> repository, IEncryptionService encryptionService)
    {
        userMapper = new UserMapper();
        _repository = repository;
        _encryptionService = encryptionService;
    }

    public Task<IEnumerable<User>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserById(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<User> RegisterUser(UserRegisterDto user)
    {
        try
        {
            var users = await _repository.GetAll();
            if (users.Any(u => u.Username == user.Username))
            {
                throw new Exception("User already exists");
            }
            var encryptedData = await _encryptionService.EncryptData(new EncryptModel
            {
                Data = user.Password
            });
            if (encryptedData.EncryptedData == null || encryptedData.HashKey == null)
            {
                throw new Exception("Encryption failed");
            }
            var newUser = userMapper.MapAddUser(user, encryptedData);
            if (newUser == null)
            {
                throw new Exception("Mapping user failed");
            }
            var addedUser = await _repository.Add(newUser);
            if (addedUser == null)
            {
                throw new Exception("Adding user failed");
            }
            return newUser;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error validating user: {ex.Message}");
        }


    }
}