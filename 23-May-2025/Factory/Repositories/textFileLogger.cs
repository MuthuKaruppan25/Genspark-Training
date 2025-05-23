using System;
using System.IO;
using Factory.Interfaces;

namespace Factory.Repositories;
public class TextFileLogger : ILogger
{
    private readonly string _filePath = "TextLog.txt";

    public TextFileLogger()
    {
        if (!File.Exists(_filePath))
            File.Create(_filePath).Dispose();
    }

    public void Write(string message)
    {
        File.AppendAllText(_filePath, message + Environment.NewLine);
    }

    public string Read()
    {
        return File.ReadAllText(_filePath);
    }
}
