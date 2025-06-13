using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.AspNetCore.SignalR;

public class SeekerNotificationService : ISeekerNotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IRepository<Guid,Seeker> _repository;

    public SeekerNotificationService(IHubContext<NotificationHub> hubContext, IRepository<Guid,Seeker> repository)
    {
        _hubContext = hubContext;
        _repository = repository;
    }

    public async Task NotifySeekersAsync(string title, DateTime lastDate)
    {
        var seekers = await _repository.GetAll();
        string formattedDate = lastDate.ToString("dd MMMM yyyy"); 
        string message = $"{title} - Last Date: {formattedDate}";

        foreach (var seeker in seekers)
        {
            if (!string.IsNullOrEmpty(seeker.ConnectionId))
            {
                await _hubContext.Clients.Client(seeker.ConnectionId)
                    .SendAsync("ReceiveMessage", title, formattedDate);
            }
        }
    }
}
