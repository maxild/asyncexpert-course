using Moq;
using ThreadPoolExercises.Core;
using Xunit;

namespace ThreadPoolExercises.Tests;

public class ExecuteOnThreadTests
{
    [Fact]
    public void RunningOnDifferentThreadTest()
    {
        var mainThreadId = Environment.CurrentManagedThreadId;
        var testThreadId = 0;
        bool? testThreadIsFromPool = null;

        ThreadingHelpers.ExecuteOnThread(
            () =>
            {
                Thread.Sleep(100);
                testThreadId = Environment.CurrentManagedThreadId;
                testThreadIsFromPool = Thread.CurrentThread.IsThreadPoolThread;
            }, repeats: 1);

        Assert.NotEqual(0, testThreadId);
        Assert.NotEqual(mainThreadId, testThreadId);
        Assert.NotNull(testThreadIsFromPool);
        Assert.False(testThreadIsFromPool);
    }

    [Fact]
    public void ImmediateCancellationTest()
    {
        var errorActionMock = new Mock<Action<Exception>>();

        CancellationTokenSource cts = new();
        cts.Cancel();

        ThreadingHelpers.ExecuteOnThread(
            () => Thread.Sleep(100),
            repeats: 3,
            errorAction: errorActionMock.Object,
            cts.Token);

        errorActionMock.Verify(m
            => m(It.IsAny<OperationCanceledException>()), Times.Once);
    }

    [Fact]
    public void TimeoutCancellationTest()
    {
        var errorActionMock = new Mock<Action<Exception>>();

        CancellationTokenSource cts = new();
        cts.CancelAfter(200);

        ThreadingHelpers.ExecuteOnThread(
            () => Thread.Sleep(100),
            repeats: 10,
            errorAction: errorActionMock.Object,
            cts.Token);

        errorActionMock.Verify(m
            => m(It.IsAny<OperationCanceledException>()), Times.Once);
    }

    [Fact]
    public void ExceptionHandlingWhenErrorActionProvidedTest()
    {
        var errorActionMock = new Mock<Action<Exception>>();

        ThreadingHelpers.ExecuteOnThread(
            () => throw new NullReferenceException(),
            repeats: 10,
            errorAction: errorActionMock.Object);

        errorActionMock.Verify(m
            => m(It.IsAny<NullReferenceException>()), Times.Once);
    }

    [Fact]
    public void ExceptionHandlingWhenErrorActionMissing()
    {
        ThreadingHelpers.ExecuteOnThread(
            () => throw new NullReferenceException(),
            repeats: 10);

        // This should simply not kill the test runner.
    }

    [Fact]
    public void ReferencePassingTest()
    {
        var data = new DataBag();

        ThreadingHelpers.ExecuteOnThread(
            () => data.X++,
            repeats: 10);

        Assert.Equal(10, data.X);
    }

    class DataBag
    {
        public int X;
    }
}
