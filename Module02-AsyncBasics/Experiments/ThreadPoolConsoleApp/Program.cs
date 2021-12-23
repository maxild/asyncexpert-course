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

Console.ReadLine();

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
        // 2_000  - some queuing, because we have 12 logical cores (10 * 2000 per second, we can process  12 * 1000ms per second)
        // 60_000 - masive queuing, big increase in #worker threads
        const int SINGLE_WORK_ITEM_DELAYS_MS = 1_000;

        int cursorLeft = state == null ? 0 : (int)state;

        Console.CursorLeft = cursorLeft;
        Console.CursorTop = 3; // fourth line

        // clear current line
        if (cursorLeft == 0)
        {
            // Console.SetCursorPosition(0, 3);
            Console.Write(new string(' ', Console.BufferWidth));
            // Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, 3);
        }

        Console.Write(".");

        // How long does the work take (simulate work via sleep)
        Thread.Sleep(SINGLE_WORK_ITEM_DELAYS_MS);
    }
}
