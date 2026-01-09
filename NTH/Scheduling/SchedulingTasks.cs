//using Hangfire;

namespace NTH.Scheduling;

public class SchedulingTasks
{
    //[AutomaticRetry(Attempts = 1)]
    public static void Writeline()
    {
        Console.WriteLine("Scheduling Task Writeline");
    }

    public static void ThrowException()
    {
        throw new Exception("Scheduling Task Throwing");
    }
}
