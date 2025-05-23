using System;
using Factory.Interfaces;
namespace Factory.Repositories;
public class ConsoleLogger : ILogger
{
    public void Write(string message)
    {
        Console.WriteLine($"[Console] {message}");
    }

    public string Read()
    {
        return "ConsoleLogger does not support reading logs.";
    }
}
