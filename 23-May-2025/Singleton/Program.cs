using System;
using System.Collections.Generic;
using Singleton;
using Singleton.Service;
class Program
{
    static void Main(string[] args)
    {
        string filePath = "Log.txt";
        FileManager fileManager = FileManager.GetInstance(filePath);
        Console.WriteLine("Singleton File Manager Example");
        Console.WriteLine("Enter a message to log (or 'exit' to quit):");
        string input = Console.ReadLine();
        while (input.ToLower() != "exit")
        {
            fileManager.WriteToFile(input);
            Console.WriteLine("Message logged. Enter another message (or 'exit' to quit):");
            input = Console.ReadLine();
        }
        Console.WriteLine("Exiting...");
        string content = fileManager.ReadFromFile();
        Console.WriteLine("Content of the file:");
        Console.WriteLine(content);
    }
}