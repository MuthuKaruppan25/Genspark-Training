using System;
namespace Factory.Services;
using Factory.Interfaces;
public abstract class LoggerFactory
{
    public abstract ILogger GetLogger();
    public void WriteLog(string message)
    {
        ILogger logger = GetLogger();
        logger.Write(message);
    }
    public string ReadLog()
    {
        ILogger logger = GetLogger();
        return logger.Read();
    }
}
