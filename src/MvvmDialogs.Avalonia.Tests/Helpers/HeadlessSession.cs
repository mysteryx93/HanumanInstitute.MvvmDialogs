namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

/// <summary>
/// Owns a headless dispatcher session and the windows it opens for a test.
/// </summary>
public class HeadlessSession : IAsyncDisposable
{
    private readonly Stack<Window> _windows = new();

    /// <summary>
    /// Returns the Avalonia HeadlessUnitTestSession.
    /// </summary>
    public HeadlessUnitTestSession Session => field ??= HeadlessUnitTestSession.StartNew(typeof(HeadlessAppBuilder));

    // Shows a window during Run and registers it for teardown in the same dispatch.
    internal Window ShowWindow(Window window)
    {
        _windows.Push(window);
        window.Show();
        return window;
    }

    /// <summary>
    /// Dispatches a synchronous test to the thread that owns its controls.
    /// </summary>
    public Task Run(Action action) =>
        Run(() =>
        {
            action();
            return Task.CompletedTask;
        });

    /// <summary>
    /// Keeps an asynchronous test on the thread that owns its controls.
    /// </summary>
    public Task Run(Func<Task> action) =>
        Session.Dispatch(
            async () =>
            {
                try { await action(); }
                finally { CloseWindows(); }
                return true;
            },
            TestContext.Current.CancellationToken);

    /// <summary>
    /// Stops the dispatcher when the owning test instance is disposed.
    /// </summary>
    public ValueTask DisposeAsync() => Session.DisposeAsync();

    // Close newer windows first, including popups owned by earlier windows.
    private void CloseWindows()
    {
        while (_windows.TryPop(out var window))
        {
            if (window.IsVisible) { window.Close(); }
        }
    }
}
