
public class LogoutRequestDto
{
    [EmailValidation]
    public string username { get; set; } = string.Empty;
}