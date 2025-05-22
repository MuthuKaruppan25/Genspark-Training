public class WhatsAppNotifier : INotifier
{
    public void Notify(string studentName, List<SubjectMark> marks, List<string> metrics)
    {
        Console.WriteLine($"\n💬 WhatsApp message sent to {studentName}:");
        foreach (var metric in metrics)
            Console.WriteLine(metric);
    }
}