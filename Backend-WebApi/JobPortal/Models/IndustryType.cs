

namespace JobPortal.Models;

public class IndustryType
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public ICollection<Company>? companies { get; set; }

}