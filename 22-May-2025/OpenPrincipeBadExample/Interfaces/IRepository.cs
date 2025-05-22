using System;
using System.Collections.Generic;
using System.Linq;

public interface IGradingStrategy
{
    double Calculate(Dictionary<string, int> marks);
}
