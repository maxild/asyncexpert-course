using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using ThreadPoolExercises.Core;

namespace ThreadPoolExercises.Benchmarks;

public class ThreadingHelpersBenchmarks
{
    private readonly SHA256 _sha256 = SHA256.Create();
    private byte[] _data;

    [GlobalSetup]
    public void Setup()
    {
        _data = new byte[1000];
        new Random(42).NextBytes(_data);
    }

    [Benchmark]
    public void ExecuteSynchronously() => _sha256.ComputeHash(_data);

    [Benchmark]
    public void ExecuteOnThread()
    {
        ThreadingHelpers.ExecuteOnThread(() => _sha256.ComputeHash(_data), repeats: 1);
    }

    [Benchmark]
    public void ExecuteOnThreadPool()
    {
        ThreadingHelpers.ExecuteOnThreadPool(() => _sha256.ComputeHash(_data), repeats: 1);
    }
}
