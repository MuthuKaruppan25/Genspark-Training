namespace Factory.Services;
using Factory.Interfaces;
using Factory.Repositories;
public class LogLoggerFactory : LoggerFactory
{
    public override ILogger GetLogger()
    {
        return new LogFileLogger();
    }
}
