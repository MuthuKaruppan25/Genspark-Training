using System;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter 9 Numbers (1 to 9) Separated by Comma(,) for 9 x 9 Sudoku:");
        int[][] board = new int[9][];
        for (int i = 0; i < 9; i++)
            board[i] = new int[9];
        getInput(board);
        Console.WriteLine("\nThe 9 x 9 Sudoku board is:");
        display(board);
        if (IsValidSudoku(board))
            Console.WriteLine("\nThe Sudoku board is VALID.");
        else
            Console.WriteLine("\nThe Sudoku board is INVALID.");

    }
    static bool IsValidSudoku(int[][] board)
    {
        for (int i = 0; i < 9; i++)
        {
            HashSet<int> rowSet = new HashSet<int>();
            HashSet<int> colSet = new HashSet<int>();
            HashSet<int> boxSet = new HashSet<int>();
            for (int j = 0; j < 9; j++)
            {

                if (!rowSet.Add(board[i][j]))
                    return false;


                if (!colSet.Add(board[j][i]))
                    return false;

                int rowIndex = 3 * (i / 3) + j / 3;
                int colIndex = 3 * (i % 3) + j % 3;
                int boxVal = board[rowIndex][colIndex];
                if (!boxSet.Add(boxVal)) return false;
            }
        }
        return true;
    }

    static void display(int[][] board)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Console.Write(board[i][j] + " ");
            }
            Console.WriteLine();
        }
    }

    static void getInput(int[][] board)
    {
        for (int i = 0; i < 9; i++)
        {
            string? inputLine = Console.ReadLine();
            string input = inputLine!.Trim();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid input. Try again.");
                getInput(board);
                return;

            }
            string[] inputRow = input.Split(',');
            if (inputRow.Length != 9)
            {
                Console.WriteLine("Invalid input. Please enter exactly 9 numbers.");
                getInput(board);
                return;

            }
            else
            {
                for (int j = 0; j < 9; j++)
                {
                    if (!int.TryParse(inputRow[j], out board[i][j]) || board[i][j] < 1 || board[i][j] > 9)
                    {
                        Console.WriteLine("Invalid input. Please enter numbers between 1 and 9.");
                        getInput(board);
                        return;

                    }

                }
            }

        }

    }
}