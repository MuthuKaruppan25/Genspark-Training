
using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Interfaces;

public interface IAuthenticationService
{
    Task<UserLoginResponse> AuthenticateUser(UserLoginRequest userLoginRequest);
}