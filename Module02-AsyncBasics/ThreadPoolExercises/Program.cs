using ThreadPoolExercises.Core;

Console.WriteLine($"Main thread is {Environment.CurrentManagedThreadId}");

ThreadingHelpers.ExecuteOnThread(() =>
{
    var thread = Thread.CurrentThread;
    Console.WriteLine($"Hello from thread {thread.ManagedThreadId} from a pool: {thread.IsThreadPoolThread}");
}, repeats: 3);

ThreadingHelpers.ExecuteOnThreadPool(() =>
{
    var thread = Thread.CurrentThread;
    Console.WriteLine(
        $"Hello from thread {thread.ManagedThreadId} from a pool: {thread.IsThreadPoolThread}");
}, repeats: 3);
