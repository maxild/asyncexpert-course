We have used this in `Program.cs`

```csharp
BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
```

Therefore this will run the benchmark

```bash
$ dotnet run -c Release --project ./Benchmark/Benchmark.csproj -- --filter '*Fibonacci*'
```

On Linux I get

> Failed to set up high priority. Make sure you have the right permissions. Message: Permission denied

This message means that the benchmark runner was not able to set the execution
to high CPU priority, so the benchmark will run slower, especially if other
applications are running at the same time.

If you encounter this problem, either run the command as Administrator on Windows,
 or sudo it if running on Mac or Linux.

```bash
$ sudo dotnet run -c Release --project ./Benchmark/Benchmark.csproj -- --filter '*Fibonacci*'
```

This is the documentation why `--filter` works

```bash
$ dotnet run -c Release  --project ./Benchmark/Benchmark.csproj -- --help
Benchmark 1.0.0
Copyright (C) 2021 Benchmark
USAGE:
Use Job.ShortRun for running the benchmarks:
   -j short
Run benchmarks in process:
   -i
Run benchmarks for .NET 4.7.2, .NET Core 2.1 and Mono. .NET 4.7.2 will be baseline because it was first.:
   --runtimes net472 netcoreapp2.1 Mono
Run benchmarks for .NET Core 2.0, .NET Core 2.1 and .NET Core 2.2. .NET Core 2.0 will be baseline because it was
first.:
   --runtimes netcoreapp2.0 netcoreapp2.1 netcoreapp2.2
Use MemoryDiagnoser to get GC stats:
   -m
Use DisassemblyDiagnoser to get disassembly:
   -d
Use HardwareCountersDiagnoser to get hardware counter info:
   --counters CacheMisses+InstructionRetired
Run all benchmarks exactly once:
   -f * -j Dry
Run all benchmarks from System.Memory namespace:
   -f System.Memory*
Run all benchmarks from ClassA and ClassB using type names:
   -f ClassA ClassB
Run all benchmarks from ClassA and ClassB using patterns:
   -f *.ClassA.* *.ClassB.*
Run all benchmarks called `BenchmarkName` and show the results in single summary:
   --filter *.BenchmarkName --join
Run selected benchmarks once per iteration:
   --runOncePerIteration
Run selected benchmarks 100 times per iteration. Perform single warmup iteration and 5 actual workload iterations:
   --invocationCount 100 --iterationCount 5 --warmupCount 1
Run selected benchmarks 250ms per iteration. Perform from 9 to 15 iterations:
   --iterationTime 250 --maxIterationCount 15 --minIterationCount 9
Run MannWhitney test with relative ratio of 5% for all benchmarks for .NET Core 2.0 (base) vs .NET Core 2.1 (diff).
.NET Core 2.0 will be baseline because it was provided as first.:
   --filter * --runtimes netcoreapp2.0 netcoreapp2.1 --statisticalTest 5%
Run benchmarks using environment variables 'ENV_VAR_KEY_1' with value 'value_1' and 'ENV_VAR_KEY_2' with value
'value_2':
   --envVars ENV_VAR_KEY_1:value_1 ENV_VAR_KEY_2:value_2

  -j, --job                (Default: Default) Dry/Short/Medium/Long or Default

  -r, --runtimes           Full target framework moniker for .NET Core and .NET. For Mono just 'Mono', for CoreRT just
                           'CoreRT'. First one will be marked as baseline!

  -e, --exporters          GitHub/StackOverflow/RPlot/CSV/JSON/HTML/XML

  -m, --memory             (Default: false) Prints memory statistics

  -t, --threading          (Default: false) Prints threading statistics

  -d, --disasm             (Default: false) Gets disassembly of benchmarked code

  -p, --profiler           Profiles benchmarked code using selected profiler. Available options: EP/ETW/CV/NativeMemory

  -f, --filter             Glob patterns

  -i, --inProcess          (Default: false) Run benchmarks in Process

  -a, --artifacts          Valid path to accessible directory

  --outliers               (Default: RemoveUpper) DontRemove/RemoveUpper/RemoveLower/RemoveAll

  --affinity               Affinity mask to set for the benchmark process

  --allStats               (Default: false) Displays all statistics (min, max & more)

  --allCategories          Categories to run. If few are provided, only the benchmarks which belong to all of them are
                           going to be executed

  --anyCategories          Any Categories to run

  --attribute              Run all methods with given attribute (applied to class or method)

  --join                   (Default: false) Prints single table with results for all benchmarks

  --keepFiles              (Default: false) Determines if all auto-generated files should be kept or removed after
                           running the benchmarks.

  --noOverwrite            (Default: false) Determines if the exported result files should not be overwritten (be
                           default they are overwritten).

  --counters               Hardware Counters

  --cli                    Path to dotnet cli (optional).

  --packages               The directory to restore packages to (optional).

  --coreRun                Path(s) to CoreRun (optional).

  --monoPath               Optional path to Mono which should be used for running benchmarks.

  --clrVersion             Optional version of private CLR build used as the value of COMPLUS_Version env var.

  --coreRtVersion          Optional version of Microsoft.DotNet.ILCompiler which should be used to run with CoreRT.
                           Example: "1.0.0-alpha-26414-01"

  --ilcPath                Optional IlcPath which should be used to run with private CoreRT build.

  --launchCount            How many times we should launch process with target benchmark. The default is 1.

  --warmupCount            How many warmup iterations should be performed. If you set it, the minWarmupCount and
                           maxWarmupCount are ignored. By default calculated by the heuristic.

  --minWarmupCount         Minimum count of warmup iterations that should be performed. The default is 6.

  --maxWarmupCount         Maximum count of warmup iterations that should be performed. The default is 50.

  --iterationTime          Desired time of execution of an iteration in milliseconds. Used by Pilot stage to estimate
                           the number of invocations per iteration. 500ms by default

  --iterationCount         How many target iterations should be performed. By default calculated by the heuristic.

  --minIterationCount      Minimum number of iterations to run. The default is 15.

  --maxIterationCount      Maximum number of iterations to run. The default is 100.

  --invocationCount        Invocation count in a single iteration. By default calculated by the heuristic.

  --unrollFactor           How many times the benchmark method will be invoked per one iteration of a generated loop.
                           16 by default

  --strategy               The RunStrategy that should be used. Throughput/ColdStart/Monitoring.

  --platform               The Platform that should be used. If not specified, the host process platform is used
                           (default). AnyCpu/X86/X64/Arm/Arm64.

  --runOncePerIteration    (Default: false) Run the benchmark exactly once per iteration.

  --info                   (Default: false) Print environment information.

  --list                   (Default: Disabled) Prints all of the available benchmark names. Flat/Tree

  --disasmDepth            (Default: 1) Sets the recursive depth for the disassembler.

  --disasmDiff             (Default: false) Generates diff reports for the disassembler.

  --buildTimeout           Build timeout in seconds.

  --stopOnFirstError       (Default: false) Stop on first error.

  --statisticalTest        Threshold for Mann-Whitney U Test. Examples: 5%, 10ms, 100ns, 1s

  --disableLogFile         Disables the logfile.

  --maxWidth               Max paramter column width, the default is 20.

  --envVars                Colon separated environment variables (key:value)

  --memoryRandomization    Specifies whether Engine should allocate some random-sized memory between iterations. It
                           makes [GlobalCleanup] and [GlobalSetup] methods to be executed after every iteration.

  --wasmEngine             Full path to a java script engine used to run the benchmarks, used by Wasm toolchain.

  --wasmMainJS             Path to the main.js file used by Wasm toolchain. Mandatory when using "--runtimes wasm"

  --wasmArgs               (Default: --expose_wasm) Arguments for the javascript engine used by Wasm toolchain.

  --customRuntimePack      Path to a custom runtime pack. Only used for wasm/MonoAotLLVM currently.

  --AOTCompilerPath        Path to Mono AOT compiler, used for MonoAotLLVM.

  --AOTCompilerMode        (Default: mini) Mono AOT compiler mode, either 'mini' or 'llvm'

  --runtimeSrcDir          Path to a local copy of dotnet/runtime. . Used by the WASM toolchain when AOTCompilerMode is
                           'wasm'.

  --help                   Display this help screen.

  --version                Display version information.
```
