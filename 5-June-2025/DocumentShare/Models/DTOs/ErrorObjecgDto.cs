namespace SecondWebApi.Models.Dtos;

public class ErrorObjectDto
{
    public int ErrorNumber { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}