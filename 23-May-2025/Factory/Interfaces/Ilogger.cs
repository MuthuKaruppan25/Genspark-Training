namespace Factory.Interfaces;
using System;
public interface ILogger
{
    void Write(string message);
    string Read();
}
