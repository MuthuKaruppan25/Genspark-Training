public interface IReportCardService
{
    void PrintReport(ReportCard reportCard);
    void SendEmail(ReportCard reportCard);
    void SendSMS(ReportCard reportCard);
    void SendWhatsApp(ReportCard reportCard);
}

// 🚫 Email service forced to implement unrelated methods
public class EmailService : IReportCardService
{
    public void PrintReport(ReportCard reportCard)
    {
        // Not needed - violates ISP
        throw new NotImplementedException();
    }

    public void SendEmail(ReportCard reportCard)
    {
        Console.WriteLine($"📧 Email sent to {reportCard.StudentName}");
    }

    public void SendSMS(ReportCard reportCard)
    {
        // Not needed - violates ISP
        throw new NotImplementedException();
    }

    public void SendWhatsApp(ReportCard reportCard)
    {
        // Not needed - violates ISP
        throw new NotImplementedException();
    }
}

// 🚫 Printer service forced to implement messaging methods
public class PrinterService : IReportCardService
{
    public void PrintReport(ReportCard reportCard)
    {
        Console.WriteLine($"🖨️ Printing report for {reportCard.StudentName}");
    }

    public void SendEmail(ReportCard reportCard)
    {
        throw new NotImplementedException();
    }

    public void SendSMS(ReportCard reportCard)
    {
        throw new NotImplementedException();
    }

    public void SendWhatsApp(ReportCard reportCard)
    {
        throw new NotImplementedException();
    }
}