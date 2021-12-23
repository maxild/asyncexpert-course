Console.Clear();

ThreadPool.GetMinThreads(out int minWorkerThreads, out int minIocpThreads);
ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxIocpThreads);
Console.WriteLine($"Worker threads (min: {minWorkerThreads}, max: {maxWorkerThreads})");
Console.WriteLine($"IOCP threads (min: {minIocpThreads}, max: {maxIocpThreads})");

var statsThread = new Thread(PrintThreadPoolStats)
{
    IsBackground = true
};
statsThread.Start();
var spawningThread = new Thread(SpawnWork)
{
    IsBackground = true
};
spawningThread.Start();

// fetching the cursor position (Console.CursorTop getter) and Console.Read
// take the same lock, on Linux
//      Console.ReadLine();
// See https://github.com/dotnet/runtime/issues/49301
// See https://github.com/dotnet/runtime/blob/ec1897724f6dc5ab49eebde5f35292493e1bed7b/src/libraries/System.Console/src/System/ConsolePal.Unix.cs#L470-L475
Console.CursorTop = 5;
Console.WriteLine($"Work Item Delay {Constants.SINGLE_WORK_ITEM_DELAYS_MS} ms. Press Ctrl+C to terminate");
// Cannot use Readline on Linux...so we spin
while (true)
{
    Thread.Sleep(1000);
}

static void PrintThreadPoolStats()
{
    while (true)
    {
        Console.CursorLeft = 0;
        Console.CursorTop = 2; // third line
        ThreadPool.GetAvailableThreads(out int workerThreads, out int iocpThreads);
        Console.WriteLine($"Current: {ThreadPool.ThreadCount}, Queued: {ThreadPool.PendingWorkItemCount}, Done: {ThreadPool.CompletedWorkItemCount}, Worker: {workerThreads}, IOCP: {iocpThreads}.");
        Thread.Sleep(100);
    }
}

static void SpawnWork()
{
    // endless loop of queuing work
    int cursorLeft = 0;
    while (true)
    {
        Thread.Sleep(100); // queue 10 work items per second
        ThreadPool.QueueUserWorkItem(DoWork, cursorLeft);
        cursorLeft += 1;
        cursorLeft %= 40;
    }

    static void DoWork(object? state)
    {


        int cursorLeft = state == null ? 0 : (int)state;

        Console.CursorLeft = cursorLeft;
        Console.CursorTop = 3; // fourth line

        // clear current line
        if (cursorLeft == 0)
        {
            // use the carriage return to go to the beginning of the line, then print
            // as many spaces as the console is width and returns to the beginning of the
            // line again
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");

            // Console.SetCursorPosition(0, 3);
            // Console.Write(new string(' ', Console.BufferWidth));
            // Console.Write(new string(' ', Console.WindowWidth));
            // In your case, instead of Console.SetCursorPosition(0, Console.CursorTop); try writing a \r.
            // Console.SetCursorPosition(0, 3);
        }

        Console.Write(".");

        // How long does the work take (simulate work via sleep)
        Thread.Sleep(Constants.SINGLE_WORK_ITEM_DELAYS_MS);
    }
}

static class Constants
{
    // 2_000  - some queuing, because we have 12 logical cores (10 * 2000 per second, we can process  12 * 1000ms per second)
    // 60_000 - masive queuing, big increase in #worker threads
    public const int SINGLE_WORK_ITEM_DELAYS_MS = 60_000;
}
