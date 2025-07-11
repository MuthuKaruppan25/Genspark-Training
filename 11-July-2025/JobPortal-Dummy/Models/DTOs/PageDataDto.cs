
using System.ComponentModel.DataAnnotations;

public class PageDataDto
{
    [Required]
    public int pageNumber { get; set; }
    [Required]
    public int pageSize { get; set; }
}