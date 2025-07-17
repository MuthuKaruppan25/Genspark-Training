using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.AspNetCore.SignalR;

public class SeekerNotificationService : ISeekerNotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IRepository<Guid, Seeker> _repository;

    public SeekerNotificationService(IHubContext<NotificationHub> hubContext, IRepository<Guid, Seeker> repository)
    {
        _hubContext = hubContext;
        _repository = repository;
    }

    public async Task NotifySeekersAsync(JobPostRegisterResponseDto post)
    {
        var seekers = await _repository.GetAll();
        Console.WriteLine("Nofification", post);
        foreach (var seeker in seekers)
        {
            if (seeker.guid != Guid.Empty)
            {
                  Console.WriteLine("Nofification", post);
                await _hubContext.Clients.Group(seeker.guid.ToString())
                    .SendAsync("ReceiveMessage", post);
            }
        }
    }
}
