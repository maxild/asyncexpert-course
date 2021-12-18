using System.Reflection;
using BenchmarkDotNet.Running;

// See also https://benchmarkdotnet.org/articles/guides/how-to-run.html
BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
