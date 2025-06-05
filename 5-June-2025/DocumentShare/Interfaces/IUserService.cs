

using DocumentShare.Models;

public interface IUserService
{
    Task<User> RegisterUser(UserRegisterDto user);
    Task<User> GetUserById(int userId);
    Task<IEnumerable<User>> GetAllUsers();
}