namespace Factory.Services;
using Factory.Interfaces;
using Factory.Repositories;
public class TextLoggerFactory : LoggerFactory
{
    public override ILogger GetLogger()
    {
        return new TextFileLogger();
    }
}
