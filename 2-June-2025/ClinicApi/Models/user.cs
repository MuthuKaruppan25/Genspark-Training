

using System.ComponentModel.DataAnnotations;
using SecondWebApi.Models;

public class User
{
    [Key]
    public string username { get; set; } = string.Empty;
    public string role { get; set; } = string.Empty;
    public byte[]? password { get; set; }
    public byte[]? HashKey { get; set; }
    public Patient? patient { get; set; }
    public Doctor? doctor { get; set; }

}