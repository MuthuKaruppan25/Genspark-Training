using Microsoft.AspNetCore.SignalR;
using JobPortal.Interfaces;
using JobPortal.Models;

public class NotificationHub : Hub
{
    private readonly IRepository<Guid, Seeker> _repository;

    public NotificationHub(IRepository<Guid, Seeker> repository)
    {
        _repository = repository;
    }

    public override async Task OnConnectedAsync()
    {
        var seekerId = Context.GetHttpContext()?.Request.Query["seekerId"];

        if (!string.IsNullOrEmpty(seekerId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, seekerId!);
            Console.WriteLine($"Seeker {seekerId} added to group with connection {Context.ConnectionId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var seekerId = Context.GetHttpContext()?.Request.Query["seekerId"];
        if (!string.IsNullOrEmpty(seekerId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, seekerId!);
            Console.WriteLine($"Seeker {seekerId} removed from group");
        }

        await base.OnDisconnectedAsync(exception);
    }
}
