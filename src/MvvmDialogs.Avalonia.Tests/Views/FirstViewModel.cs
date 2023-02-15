using System.ComponentModel;
using ReactiveUI;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests.Views;

public class FirstViewModel : ReactiveObject, ICloseable, IActivable, IViewLoaded, IViewClosing, IViewClosed
{
    public event EventHandler RequestClose;
    public event EventHandler RequestActivate;
    
    public void OnRequestClose() => RequestClose?.Invoke(this, EventArgs.Empty);

    public void OnRequestActivate() => RequestActivate?.Invoke(this, EventArgs.Empty);

    public int LoadedCount { get; private set; }
    public int ClosingCount { get; private set; }
    public int ClosedCount { get; private set; }

    public void ResetCounters()
    {
        LoadedCount = 0;
        ClosingCount = 0;
        ClosedCount = 0;
    }

    public virtual void OnLoaded() => LoadedCount++;
    public virtual void OnClosing(CancelEventArgs e) => ClosingCount++;
    public virtual Task OnClosingAsync(CancelEventArgs e) => Task.CompletedTask;
    public virtual void OnClosed() => ClosedCount++;
}
