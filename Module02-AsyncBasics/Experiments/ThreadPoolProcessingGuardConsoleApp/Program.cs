#define USE_GUARD

static class Program
{
    private static AutoResetEvent s_guardRunningFlag = new(false);

    public static void Main()
    {
        Console.Clear();
        Console.WriteLine($"# Processors: {Environment.ProcessorCount}");

        Console.CursorVisible = false;

        // NOTE: One Pool Thread is reserved with monitoring the queue length (processing guard thread)
        //       On a 12 core machine we therefore can process 11 work items at a time using the remaining
        //       pool threads.

        // To make queueing much easier
        ThreadPool.SetMinThreads(workerThreads: Environment.ProcessorCount, completionPortThreads: Environment.ProcessorCount);
        // You cannot go lower than these numbers
        ThreadPool.SetMaxThreads(workerThreads: Environment.ProcessorCount, completionPortThreads: 1000);

        ThreadPool.GetMinThreads(out int minWorkerThreads, out int minIocpThreads);
        ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxIocpThreads);
        Console.WriteLine($"Worker threads (min: {minWorkerThreads}, max: {maxWorkerThreads})");
        Console.WriteLine($"IOCP threads (min: {minIocpThreads}, max: {maxIocpThreads})");

#if USE_GUARD
        ThreadPool.QueueUserWorkItem(ProcessingGuard);
        s_guardRunningFlag.WaitOne();
#endif

        // Enqueue 30 work items
        for (int i = 0; i < 30; i++)
            ThreadPool.QueueUserWorkItem(DoWork, i);

        // App just ends, no waiting ny main thread
        // Console.ReadKey();

        Console.CursorVisible = true;
    }

    static void DoWork(object? state)
    {
        var previousState = Thread.CurrentThread.IsBackground ? "B" : "F";
        // This will also prevent worker threads from being killed before termination of below loop
        Thread.CurrentThread.IsBackground = false;
        const int ITER = 10; // 10 times each second
        int id = (int)state!;
        Debug(0, id + 5, $"{id:D2}{previousState}");
        // Simulate work for 10 seconds
        for (int i = 0; i < ITER; i++)
        {
            Debug(i + 5, id + 5, ".");
            Thread.Sleep(1000);
        }
    }

    static void ProcessingGuard(object? state)
    {
        Thread.CurrentThread.IsBackground = false;
        s_guardRunningFlag.Set(); // signal that main thread can enqueue work

        const int EmptyQueueThresholdMilliseconds = 3_000;
        const int LoopDelayMilliseconds = 100;

        int emptyQueueCounter = 0;
        int emptyQueueThreshold = EmptyQueueThresholdMilliseconds / LoopDelayMilliseconds;
        while (true)
        {
            long queue = ThreadPool.PendingWorkItemCount;
            Debug(0, 4, $"Guard Q:{queue:D2}, C:{emptyQueueCounter:D2}");
            if (queue == 0)
            {
                // sum up the number of times empty queue is observed
                emptyQueueCounter += 1;
                if (emptyQueueCounter >= emptyQueueThreshold)
                    break; // observed 30 times = 3 seconds
            }
            else
            {
                // reset the counter
                emptyQueueCounter = 0;
            }
            Thread.Sleep(LoopDelayMilliseconds);
        }
        Debug(0, 0, "G: done!          ");
    }

    static void Debug(int x, int y, string s)
    {
        Console.CursorLeft = x;
        Console.CursorTop = y;
        Console.Write(s);
    }
}
