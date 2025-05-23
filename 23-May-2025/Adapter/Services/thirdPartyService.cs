namespace Adapter.Services;
public class LegacyCSVReportTool
{
    public void PrintReport(string csvData)
    {
        Console.WriteLine("[Legacy Report]");
        Console.WriteLine(csvData);
    }
}
