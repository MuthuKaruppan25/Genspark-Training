using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;

class Result
{

    /*
     * Complete the 'timeConversion' function below.
     *
     * The function is expected to return a STRING.
     * The function accepts STRING s as parameter.
     */

    public static string timeConversion(string s)
    {
        string hours = s.Substring(0,2);
        string format = s.Substring(s.Length - 2);
        string Minutes = s.Substring(2, s.Length - 4);
        int numhours = Convert.ToInt32(hours);
        if(format == "AM")
        {
            if(numhours == 12)
            numhours = 0;
        }
        else if(format == "PM")
        {
            if(numhours != 12)
            numhours += 12;
        }
        string convertedHours = numhours.ToString("D2");
        string time = convertedHours + Minutes;
        return time;
    }

}

class Solution
{
    public static void Main(string[] args)
    {
        TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        string s = Console.ReadLine();

        string result = Result.timeConversion(s);

        textWriter.WriteLine(result);

        textWriter.Flush();
        textWriter.Close();
    }
}
