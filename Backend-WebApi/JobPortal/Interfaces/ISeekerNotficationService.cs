
public interface ISeekerNotificationService
{
    Task NotifySeekersAsync(string title, DateTime lastDate);
}