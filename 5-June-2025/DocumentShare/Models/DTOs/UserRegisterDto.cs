

using DocumentShare.Misc;

public class UserRegisterDto
{   
    [UsernameValidation]
    public string Username { get; set; } = string.Empty;
    [PasswordValidation]
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    [TextValidator]
    public string Name { get; set; } = string.Empty;
    [TextValidator]
    public string Department { get; set; } = string.Empty;

}