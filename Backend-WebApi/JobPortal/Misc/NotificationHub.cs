
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    private readonly IRepository<Guid, Seeker> _repository;


    public NotificationHub(IRepository<Guid, Seeker> repository)
    {
        _repository = repository;
    }
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
    public string GetConnectionId()
    {
        return Context.ConnectionId;
    }

    public async Task SendToConnection(string title, DateTime lastDate)
    {
        var seekers = await _repository.GetAll();
        string formattedDate = lastDate.ToString("dd MMMM yyyy"); 
        string message = $"{title} - Last Date: {formattedDate}";

        foreach (var seeker in seekers)
        {
            if (!string.IsNullOrEmpty(seeker.ConnectionId))
            {
                Console.WriteLine("Hello" + seeker.ConnectionId);
                await Clients.Client(seeker.ConnectionId).SendAsync("ReceiveMessage", message);
            }
        }
    }

}