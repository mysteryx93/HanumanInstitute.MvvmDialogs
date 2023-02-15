using System.ComponentModel;
using ReactiveUI;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests.Views;

public class FirstViewModel : ReactiveObject, ICloseable, IActivable, IViewLoaded, IViewClosing, IViewClosed
{
    public event EventHandler RequestClose;
    public event EventHandler RequestActivate;
    
    public void OnRequestClose() => RequestClose?.Invoke(this, EventArgs.Empty);

    public void OnRequestActivate() => RequestActivate?.Invoke(this, EventArgs.Empty);
    
    public event EventHandler ViewLoaded;
    public void RaiseViewLoaded() => ViewLoaded?.Invoke(this, EventArgs.Empty);
    
    public event EventHandler<CancelEventArgs> ViewClosing;
    public void RaiseViewClosing(CancelEventArgs e) => ViewClosing?.Invoke(this, e);
    public Task OnViewClosingAsync(CancelEventArgs e) => Task.CompletedTask;
    
    public event EventHandler ViewClosed;
    public void RaiseViewClosed() => ViewClosed?.Invoke(this, EventArgs.Empty);
}
