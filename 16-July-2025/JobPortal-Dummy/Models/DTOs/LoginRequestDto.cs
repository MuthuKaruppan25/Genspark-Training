
public class LoginRequestDto
{
    [EmailValidation]
    public string Username { get; set; } = string.Empty;
    [PasswordValidation]
    public string Password { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
}