
namespace DocumentShare.Models;

public class EncryptModel
{
    public string? Data { get; set; }
    public byte[]? EncryptedData { get; set; }
    public byte[]? HashKey { get; set; }
}