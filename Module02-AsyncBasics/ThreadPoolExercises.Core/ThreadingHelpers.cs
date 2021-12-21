namespace ThreadPoolExercises.Core;

public class ThreadingHelpers
{
    public static void ExecuteOnThread(Action action, int repeats, Action<Exception>? errorAction = null,
        CancellationToken token = default)
    {
        // * Create a thread and execute there `action` given number of `repeats` - waiting for the execution!
        //   HINT: you may use `Join` to wait until created Thread finishes
        // * In a loop, check whether `token` is not cancelled
        // * If an `action` throws and exception (or token has been cancelled) - `errorAction` should be invoked (if provided)
        var thread = new Thread(() =>
        {
            try
            {
                while (!token.IsCancellationRequested && repeats > 0)
                {
                    action();
                    repeats -= 1;
                }

                if (errorAction != null && token.IsCancellationRequested)
                    errorAction(new OperationCanceledException(token));
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex);
            }
        })
        {
            IsBackground = true
        };

        thread.Start();

        // block the calling thread until the created thread terminates
        thread.Join();
    }

    public static void ExecuteOnThreadPool(Action action, int repeats, Action<Exception>? errorAction = null,
        CancellationToken token = default)
    {
        // * Queue work item to a thread pool that executes `action` given number of `repeats` - waiting for the execution!
        //   HINT: you may use `AutoResetEvent` to wait until the queued work item finishes
        // * In a loop, check whether `token` is not cancelled
        // * If an `action` throws and exception (or token has been cancelled) - `errorAction` should be invoked (if provided)

        // EventWaitHandle
        var evt = new AutoResetEvent(initialState: false); // kernel object
        // we do not know when the work item (lambda) will be executed,
        // and there is no direct way to get the result (fire and forget)
        _ = ThreadPool.QueueUserWorkItem(DoWork);

        // block the calling thread until the wait handle receives a signal
        evt.WaitOne();

        // WaitCallback
        void DoWork(object? _) // no state passed when using QUWI
        {
            try
            {
                while (!token.IsCancellationRequested && repeats > 0)
                {
                    action();
                    repeats -= 1;
                }

                if (errorAction != null && token.IsCancellationRequested)
                    errorAction(new OperationCanceledException(token));
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex);
            }
            finally
            {
                evt.Set(); // signal the wait handle
            }
        }
    }
}
