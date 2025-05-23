using Adapter.Interfaces;
using Adapter.Models;
using System;
using System.Text;
namespace Adapter.Services;
public class StudentCSVAdapter : IStudentCSVAdapter
{
    public string ConvertToCSV(List<StudentMark> students)
    {
        if (students == null || students.Count == 0)
            return "No data available.";

        var csv = new StringBuilder();
        csv.AppendLine("Name,Math,Science,English");

        foreach (var s in students)
        {
            csv.AppendLine($"{s.Name},{s.Math},{s.Science},{s.English}");
        }

        return csv.ToString();
    }
}
