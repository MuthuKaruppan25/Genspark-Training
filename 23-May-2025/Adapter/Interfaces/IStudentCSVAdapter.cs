
using Adapter.Models;
namespace Adapter.Interfaces;

public interface IStudentCSVAdapter
{
    string ConvertToCSV(List<StudentMark> students);
}
