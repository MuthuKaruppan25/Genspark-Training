using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter 9 Numbers (1 to 9) Separated by Comma(,):");
        int[] row = new int[9];
        getInput(row);
        display(row);

        if (IsValidSudokuRow(row))
            Console.WriteLine("\nThe row is VALID.");
        else
            Console.WriteLine("\nThe row is INVALID.");
    }

    static void display(int[] row)
    {
        Console.WriteLine("\nThe 9 numbers are:");
        for (int i = 0; i < 9; i++)
        {
            Console.Write(row[i] + " ");
        }
    }

    static void getInput(int[] row)
    {
        string? inputLine = Console.ReadLine()!.Trim();
        if (string.IsNullOrEmpty(inputLine))
        {
            Console.WriteLine("Invalid input. Try again.");
            getInput(row);
            return;
        }

        string[] input = inputLine.Split(',');
        if (input.Length != 9)
        {
            Console.WriteLine("Invalid Input. Please enter exactly 9 numbers.");
            getInput(row);
            return;
        }

        for (int i = 0; i < 9; i++)
        {
            if (!int.TryParse(input[i], out row[i]) || row[i] < 1 || row[i] > 9)
            {
                Console.WriteLine("Invalid Input. Please enter numbers between 1 and 9.");
                getInput(row);
                return;
            }
        }
    }

    static bool IsValidSudokuRow(int[] row)
    {
        bool[] seen = new bool[10]; 

        foreach (int num in row)
        {
            if (seen[num])
                return false;
            seen[num] = true;
        }
        return true;
    }
}
