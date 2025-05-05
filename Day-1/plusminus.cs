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

 

    public static void plusMinus(List<int> arr)
    {
        double positiveCount = 0;
        double negativeCount = 0;
        double zeroCount = 0;
        int total = arr.Count;
        foreach(int num in arr)
        {
            if(num > 0)
            positiveCount+=1;
            else if(num < 0)
            negativeCount+=1;
            else
            zeroCount+=1;
        }
    Console.WriteLine((positiveCount / total).ToString("F6"));
    Console.WriteLine((negativeCount / total).ToString("F6"));
    Console.WriteLine((zeroCount / total).ToString("F6"));
        
    }

}

class Solution
{
    public static void Main(string[] args)
    {
        int n = Convert.ToInt32(Console.ReadLine().Trim());

        List<int> arr = Console.ReadLine().TrimEnd().Split(' ').ToList().Select(arrTemp => Convert.ToInt32(arrTemp)).ToList();

        Result.plusMinus(arr);
    }
}
