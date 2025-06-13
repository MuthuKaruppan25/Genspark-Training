
public class SeekerRegisterResponseDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Experience { get; set; }
    public string About { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public ICollection<string>? skills { get; set; }
}
