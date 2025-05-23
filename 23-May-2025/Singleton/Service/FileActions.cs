using System;
using System.IO;
namespace Singleton.Service;
public class FileManager
{
    private static FileManager _instance;
    private static readonly object _lockObject = new object();
    private readonly string _filePath;

    private FileManager(string path)
    {
        _filePath = path;
        if (!File.Exists(_filePath))
        {
            File.Create(_filePath).Dispose();
        }
    }
    public static FileManager GetInstance(string path)
    {
        if (_instance == null)
        {
            lock (_lockObject)
            {
                if (_instance == null)
                {
                    _instance = new FileManager(path);
                }
            }
        }
        return _instance;
    }
    public void WriteToFile(string content)
    {
        File.AppendAllText(_filePath, content + Environment.NewLine);
    }
    public string ReadFromFile()
    {
        return File.ReadAllText(_filePath);
    }
}