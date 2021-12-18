﻿using BenchmarkDotNet.Attributes;

namespace Benchmark;

[DisassemblyDiagnoser(exportCombinedDisassemblyReport: true)]
public class FibonacciCalc
{
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
        return n is 1 or 2 ? 1 : Recursive(n - 2) + Recursive(n - 1);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong RecursiveWithMemoization(ulong n)
    {
        return 0;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong Iterative(ulong n)
    {
        return 0;
    }

    public static IEnumerable<ulong> Data()
    {
        yield return 15;
        yield return 35;
    }
}
