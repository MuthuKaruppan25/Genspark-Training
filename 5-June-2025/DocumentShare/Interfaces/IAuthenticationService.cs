

namespace DocumentShare.Interfaces;

public interface IAuthenticationService
{
    Task<UserLoginResponse> AuthenticateUser(UserLoginRequest userLoginRequest);
}