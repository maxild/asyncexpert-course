using BenchmarkDotNet.Attributes;
using Xunit;

namespace Benchmark;

[MemoryDiagnoser] // It is not windows only any longer!!!
[DisassemblyDiagnoser(exportCombinedDisassemblyReport: true)]
public class FibonacciCalc
{
    private const int MAX_INDEX = 35;
    private static readonly ulong[] s_fibCache = new ulong[MAX_INDEX + 1];

    // HOMEWORK:
    // 1. Write implementations for RecursiveWithMemoization and Iterative solutions
    // 2. Add MemoryDiagnoser to the benchmark
    // 3. Run with release configuration and compare results
    // 4. Open disassembler report and compare machine code
    //
    // You can use the discussion panel to compare your results with other students

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public ulong Recursive(ulong n)
    {
        // Base Case
        //      Fib(0) = 0
        //      Fib(1) = 1
        // Inductive Case
        //      Fib(n) = Fib(n-1) + Fib(n-2), n > 1
        return n is 0 or 1
            ? n                                         // base case
            : Recursive(n - 2) + Recursive(n - 1);      // inductive case
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong RecursiveWithMemoization(ulong n)
    {
        if (n is 0 or 1)
            return n; // base case (otherwise we do not know if memoization store is initialized)

        ulong result = s_fibCache[n];
        if (result > 0)
            return result;

        // try memoize for all indexes less than n
        result = RecursiveWithMemoization(n - 1) + RecursiveWithMemoization(n - 2);
        s_fibCache[n] = result;
        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong Iterative(ulong n)
    {
        if (n is 0 or 1)
            return n;

        ulong prev = 0; // Fib(n-2), for n = 2
        ulong curr = 1; // Fib(n-1), for n = 2
        while (n-- > 1)
            // Fib(2) is calculated on first iteration
            // Fib(n) is calculated on n - 2 => the loop should run (n-2) times
            (curr, prev) = (prev + curr, curr);

        return curr;
    }

    public static IEnumerable<ulong> Data()
    {
        yield return 15;
        yield return 35;
    }
}

public class FibonacciCalcTests
{
    [Fact]
    public void Recursive()
    {
        Assert.Equal(5uL, new FibonacciCalc().Recursive(5));
    }

    [Fact]
    public void RecursiveWithMemoization()
    {
        Assert.Equal(5uL, new FibonacciCalc().RecursiveWithMemoization(5));
    }

    [Fact]
    public void Iterative()
    {
        Assert.Equal(5uL, new FibonacciCalc().Iterative(5));
    }
}
