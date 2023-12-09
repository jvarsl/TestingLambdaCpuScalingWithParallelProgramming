using Amazon.Lambda.Core;
using System.Diagnostics;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ParallelPrimeNumberProcessingTestOnLambda;
public class Function
{
    public enum Mode
    {
        Sync = 0, Async = 1, AsyncTwoTasksComputation = 2
    }

    public async Task<string> FunctionHandler(Mode? mode, ILambdaContext context)
    {
        switch (mode)
        {
            case Mode.Sync:
                DoSearchSync();
                break;
            case Mode.Async:
                await DoSearchAsync();
                break;
            case Mode.AsyncTwoTasksComputation:
                await DoSearchAsyncTwoTasksComputation();
                break;
            default:
                throw new NotImplementedException();
        }
        return "done";
    }

    private static Task DoSearchAsyncTwoTasksComputation()
    {
        Console.WriteLine($"doing {nameof(DoSearchAsyncTwoTasksComputation)}");
        var sw = new Stopwatch();
        sw.Start();
        Parallel.For(0, 2, _ => DoSearchSync());
        Console.WriteLine($"{nameof(DoSearchAsyncTwoTasksComputation)} Process time: {sw.ElapsedMilliseconds}");
        return Task.CompletedTask;
    }

    private static void DoSearchSync()
    {
        Console.WriteLine($"doing {nameof(DoSearchSync)}");
        var sw = new Stopwatch();
        sw.Start();
        var primes = GetPrimeNumbers(2, 10000000);
        Console.WriteLine($"{nameof(DoSearchSync)} Total prime numbers: {primes.Count}\nProcess time: {sw.ElapsedMilliseconds}");
    }

    private static Task DoSearchAsync()
    {
        Console.WriteLine($"doing {nameof(DoSearchAsync)}");
        var sw = new Stopwatch();
        sw.Start();
        const int numParts = 10;
        var primes = new List<int>[numParts];
        Parallel.For(0, numParts, i => primes[i] = GetPrimeNumbers(i == 0 ? 2 : i * 1000000 + 1, (i + 1) * 1000000));
        var result = primes.Sum(p => p.Count);
        Console.WriteLine($"{nameof(DoSearchAsync)} Total prime numbers: {result}\nProcess time: {sw.ElapsedMilliseconds}");
        return Task.CompletedTask;
    }

    private static List<int> GetPrimeNumbers(int minimum, int maximum)
    {
        List<int> result = new List<int>();
        for (int i = minimum; i <= maximum; i++)
        {
            if (IsPrimeNumber(i))
            {
                result.Add(i);
            }
        }
        Console.WriteLine($"returning from thread {Environment.CurrentManagedThreadId}");
        return result;
    }

    static bool IsPrimeNumber(int number)
    {
        if (number % 2 == 0)
        {
            return number == 2;
        }
        else
        {
            var topLimit = (int)Math.Sqrt(number);
            for (int i = 3; i <= topLimit; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }
    }
}

