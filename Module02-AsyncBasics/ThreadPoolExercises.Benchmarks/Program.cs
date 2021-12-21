using BenchmarkDotNet.Running;
using ThreadPoolExercises.Benchmarks;

// Run with
// $ sudo dotnet run -c Release --project ./ThreadPoolExercises.Benchmarks/ThreadPoolExercises.Benchmarks.csproj -- --filter '**'
_ = BenchmarkRunner.Run<ThreadingHelpersBenchmarks>();
