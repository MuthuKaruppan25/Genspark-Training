using System;
namespace Factory.Services;
using Factory.Interfaces;
using Factory.Repositories;
public class ConsoleLoggerFactory : LoggerFactory
{
    public override ILogger GetLogger()
    {
        return new ConsoleLogger();
    }
}
