public class PassFailEvaluation : IGradingStrategy
{
    public double Calculate(Dictionary<string, int> marks)
    {
        foreach (var mark in marks.Values)
        {
            if (mark < 35)
            {
                Console.WriteLine("Result: Fail");
                return 0;
            }
        }
        Console.WriteLine("Result: Pass");
        return 1;
    }
}
