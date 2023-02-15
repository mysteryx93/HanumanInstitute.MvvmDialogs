using System.ComponentModel;
using ReactiveUI;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests.Views;

public class FirstViewModel : ReactiveObject, ICloseable, IActivable, IViewLoaded, IViewClosing, IViewClosed
{
    public event EventHandler RequestClose;
    public event EventHandler RequestActivate;
    
    public void OnRequestClose() => RequestClose?.Invoke(this, EventArgs.Empty);

    public void OnRequestActivate() => RequestActivate?.Invoke(this, EventArgs.Empty);
    
    public event EventHandler Loaded;
    public void RaiseLoaded() => Loaded?.Invoke(this, EventArgs.Empty);
    
    public event EventHandler<CancelEventArgs> Closing;
    public void RaiseClosing(CancelEventArgs e) => Closing?.Invoke(this, e);
    public Task OnClosingAsync(CancelEventArgs e) => Task.CompletedTask;
    
    public event EventHandler Closed;
    public void RaiseClosed() => Closed?.Invoke(this, EventArgs.Empty);
}
