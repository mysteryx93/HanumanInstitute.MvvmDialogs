using HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

public class CancellableActionsTests
{
    [Fact]
    public async Task RunAsync_WhenActionThrows_RemovesCallback()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => CancellableActions.RunAsync<int>(
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
}
