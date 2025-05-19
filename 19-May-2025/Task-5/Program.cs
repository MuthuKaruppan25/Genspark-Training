using System;
class Program
{
    static void Main(string[] args)
    {
        int i=0;
        int cnt = 0;
        Console.WriteLine("Enter 10 numbers:");
        while(i < 10)
        {
            int num;
            bool isvalid =int.TryParse(Console.ReadLine(),out num);
            if(!isvalid)
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                continue;
            }
            bool isNum = CheckDivisibleby7(num);
            if(isNum)
            {
                cnt++;
            }
            i++;
        }
        if(cnt == 0)
        {
            Console.WriteLine("No numbers are divisible by 7");
        }
        else
        {
            Console.WriteLine(cnt + " numbers are divisible by 7");
        }
    }
    static bool CheckDivisibleby7(int num)
    {
        if(num % 7 == 0)
        {
            Console.WriteLine(num + " is divisible by 7");
            return true;
        }
        else
        {
            Console.WriteLine(num + " is not divisible by 7");
            return false;
        }
    }
}