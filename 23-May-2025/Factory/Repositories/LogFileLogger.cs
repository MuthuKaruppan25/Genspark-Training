using System;
using System.IO;
using Factory.Interfaces;
namespace Factory.Repositories;
public class LogFileLogger : ILogger
{
    private readonly string _filePath = "SystemLog.log";

    public LogFileLogger()
    {
        if (!File.Exists(_filePath))
            File.Create(_filePath).Dispose();
    }

    public void Write(string message)
    {
        File.AppendAllText(_filePath, $"[LOG] {message}{Environment.NewLine}");
    }

    public string Read()
    {
        return File.ReadAllText(_filePath);
    }
}
