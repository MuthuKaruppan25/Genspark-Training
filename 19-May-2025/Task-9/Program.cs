using System;
class Program
{
    static void Main(string[] args)
    {
        string secret = "GAME";
        Console.WriteLine("Welcome to the Guessing Game!");
        int attempts = 0;
        while(true)
        {
            attempts++;
            string guess = GetGuess();

            var (bulls,cows)  =  getBullsAndCows(secret, guess);
            Console.WriteLine($"Bulls: {bulls}, Cows: {cows}");
            if (bulls == 4)
            {
                Console.WriteLine($"Congratulations! You've guessed the secret word '{secret}' in {attempts} attempts.");
                break;
            }

        }

    }
    static (int bulls,int cows) getBullsAndCows(string secret, string guess)
    {
        int bulls = 0;
        int cows = 0;
        for (int i = 0; i < 4; i++)
        {
            if (secret[i] == guess[i])
            {
                bulls++;
            }
            else if (secret.Contains(guess[i]))
            {
                cows++;
            }
        }
        return (bulls, cows);
    }
 static string GetGuess()
    {
        while (true)
        {
            Console.Write("Enter your guess (4-letter word): ");
            string? guess = Console.ReadLine()!.Trim();

            if (string.IsNullOrEmpty(guess) || guess.Length != 4 || !IsValid(guess))
            {
                Console.WriteLine("Invalid input. Please enter a 4-letter word with alphabetic characters only.");
                continue;
            }

            return guess.ToUpper();
        }
    }

    static bool IsValid(string guess)
    {
        foreach (char c in guess)
        {
            if (!char.IsLetter(c))
                return false;
        }
        return true;
    }
}