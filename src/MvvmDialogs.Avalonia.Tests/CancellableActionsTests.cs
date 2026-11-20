namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

public class CancellableActionsTests
{
    [Fact]
    public async Task RunAsync_WhenActionThrows_RemovesCallback()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => CancellableActions.RunAsync<int>(
            () => throw new InvalidOperationException("show failed"),
            () => { }));

        Assert.Equal(0, CancellableActions.Count);
    }

    [Fact]
    public async Task RunAsync_WhenActionCompletes_RemovesCallback()
    {
        var result = await CancellableActions.RunAsync(() => Task.FromResult(7), () => { });

        Assert.Equal(7, result);
        Assert.Equal(0, CancellableActions.Count);
    }

    [Fact]
    public async Task CancelLast_WhenCancelCompletesSynchronously_DoesNotRemoveOtherCallbacks()
    {
        var extraCalled = false;
        var extra = new TaskCompletionSource<int>();
        var extraRun = CancellableActions.RunAsync(() => extra.Task, () => extraCalled = true);

        var inner = new TaskCompletionSource<int>();
        var run = CancellableActions.RunAsync(() => inner.Task, () => inner.TrySetResult(42));

        var cancelled = CancellableActions.CancelLast();
        var result = await run;

        Assert.True(cancelled);
        Assert.Equal(42, result);
        Assert.False(extraCalled);
        Assert.Equal(1, CancellableActions.Count);

        extra.SetResult(0);
        await extraRun;
        Assert.Equal(0, CancellableActions.Count);
    }

    [Fact]
    public async Task CancelLast_WhenSameCallbackRegisteredTwice_DoesNotRemoveEarlierRegistration()
    {
        var extra = new TaskCompletionSource<int>();
        var inner = new TaskCompletionSource<int>();
        var cancelCalls = 0;
        Action cancel = () =>
        {
            cancelCalls++;
            inner.TrySetResult(42);
        };

        var extraRun = CancellableActions.RunAsync(() => extra.Task, cancel);
        var run = CancellableActions.RunAsync(() => inner.Task, cancel);

        var cancelled = CancellableActions.CancelLast();
        var result = await run;

        Assert.True(cancelled);
        Assert.Equal(42, result);
        Assert.Equal(1, cancelCalls);
        Assert.Equal(1, CancellableActions.Count);

        extra.SetResult(0);
        await extraRun;
        Assert.Equal(0, CancellableActions.Count);
    }

    [Fact]
    public async Task CancelLast_WhenDismissedIsVetoed_AllowsAnotherAttempt()
    {
        var calls = 0;
        var gate = new TaskCompletionSource<int>();
        var run = CancellableActions.RunAsync(() => gate.Task, () => calls++);

        var first = CancellableActions.CancelLast();
        var second = CancellableActions.CancelLast();

        Assert.True(first);
        Assert.True(second);
        Assert.Equal(2, calls);
        Assert.Equal(1, CancellableActions.Count);
        gate.SetResult(0);
        await run;
        Assert.Equal(0, CancellableActions.Count);
    }

    [Fact]
    public async Task CancelLast_WhileDismissalRuns_ConsumesTheNextRequest()
    {
        var entered = new TaskCompletionSource<int>();
        var release = new TaskCompletionSource<int>();
        var gate = new TaskCompletionSource<int>();
        var calls = 0;
        var run = CancellableActions.RunAsync(
            () => gate.Task,
            () =>
            {
                calls++;
                entered.TrySetResult(0);
                release.Task.GetAwaiter().GetResult();
            });

        var first = Task.Run(() => CancellableActions.CancelLast());
        await entered.Task;
        var second = CancellableActions.CancelLast();
        release.SetResult(0);

        Assert.True(second);
        Assert.Equal(1, calls);
        Assert.True(await first);
        gate.SetResult(0);
        await run;
        Assert.Equal(0, CancellableActions.Count);
    }
}
