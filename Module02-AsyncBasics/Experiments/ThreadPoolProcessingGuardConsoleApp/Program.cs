#define USE_GUARD

static class Program
{
    private static AutoResetEvent s_guardRunningFlag = new(false);

    public static void Main()
    {
        // TODO: Maybe spin instead
        // Console.ReadKey();

        Console.Clear();
        Console.CursorVisible = false;

        // To make queueing much easier
        ThreadPool.SetMinThreads(workerThreads: 12, completionPortThreads: 12);
        // You cannot go lower than these numbers
        ThreadPool.SetMinThreads(workerThreads: Environment.ProcessorCount, completionPortThreads: 1000);


#if USE_GUARD
        ThreadPool.QueueUserWorkItem(ProcessingGuard);
        s_guardRunningFlag.WaitOne();
#endif

        // Enqueue 20 work items
        var random = new Random();
        for (int i = 0; i < 20; i++)
            ThreadPool.QueueUserWorkItem(DoWork, i);

        // App just ends, no waiting here...
        // Console.ReadKey();
    }

    static void DoWork(object? state)
    {
        var previousState = Thread.CurrentThread.IsBackground ? "B" : "F";
        // Thread.CurrentThread.IsBackground = false;
        const int ITER = 10; // 10 times each second
        int id = (int)state!;
        Debug(0, 0, $"{id:D2}{previousState}");
        for (int i = 0; i < ITER; i++)
        {
            Debug(i + 5, id + 1, ".");
            Thread.Sleep(1000);
        }
    }

    static void ProcessingGuard(object? state)
    {
        // make foreground thread to guard against closing
        Thread.CurrentThread.IsBackground = false;
        s_guardRunningFlag.Set();

        const int EmptyQueueThresholdMilliseconds = 3_000;
        const int LoopDelayMilliseconds = 100;

        int emptyQueueCounter = 0;
        int emptyQueueThreshold = EmptyQueueThresholdMilliseconds / LoopDelayMilliseconds;
        while (true)
        {
            long queue = ThreadPool.PendingWorkItemCount;
            Debug(0, 0, $"Guard Q:{queue: D2}, C:{emptyQueueCounter:D2}");
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
        Debug(0, 0, "G: done!");
    }

    static void Debug(int x, int y, string s)
    {
        Console.CursorTop = x;
        Console.CursorLeft = y;
        Console.Write(s);
    }
}
