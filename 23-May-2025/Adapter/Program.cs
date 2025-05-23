using System;
using Adapter.Models;
using Adapter.Interfaces;
using Adapter.Services;
class Program
{
    static void Main()
    {
        var students = new List<StudentMark>
        {
            new StudentMark { Name = "Alice", Math = 90, Science = 85, English = 88 },
            new StudentMark { Name = "Bob", Math = 78, Science = 82, English = 91 },
        };

        IStudentCSVAdapter adapter = new StudentCSVAdapter();
        string csvData = adapter.ConvertToCSV(students);

        LegacyCSVReportTool legacyTool = new LegacyCSVReportTool();
        legacyTool.PrintReport(csvData);
    }
}
