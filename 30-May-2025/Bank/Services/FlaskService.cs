// Services/FlaskService.cs
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BankApi.Interfaces;
using BankApi.Model.Dtos;


public class FlaskService : IFlaskService
{
    private readonly HttpClient _httpClient;

    public FlaskService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetAnswerAsync(string question)
    {
        var requestData = new { question = question };
        var response = await _httpClient.PostAsJsonAsync("http://127.0.0.1:5000/answer", requestData);

        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadFromJsonAsync<FlaskAnswerResponse>();
        return responseData?.Answer ?? "No answer received.";
    }
    public async Task<string> RetrieveAnswerAsync(string question)
    {
        var requestData = new { question = question };
        var response = await _httpClient.PostAsJsonAsync("http://127.0.0.1:5000/faq", requestData);

        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadFromJsonAsync<FlaskAnswerResponse>();
        return responseData?.Answer ?? "No answer received.";
    }


}
