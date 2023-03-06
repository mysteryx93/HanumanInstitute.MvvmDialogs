using System.ComponentModel;
using ReactiveUI;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests.Views;

public class SecondViewModel : ReactiveObject, ICloseable, IActivable, IModalDialogViewModel, IViewClosing, IViewClosed
{
    public event EventHandler RequestClose;
    public event EventHandler RequestActivate;
    
    public void OnRequestClose() => RequestClose?.Invoke(this, EventArgs.Empty);

    public void OnRequestActivate() => RequestActivate?.Invoke(this, EventArgs.Empty);
    
    public bool? DialogResult { get; set; }

    public int ClosingRaised { get; set; }
    public int ClosingAsyncRaised { get; set; }
    public int ClosedRaised { get; set; }
    public bool ClosingCancel { get; set; }
    public bool ClosingAsyncCancel { get; set; } = true;

    public void OnClosing(CancelEventArgs e)
    {
        e.Cancel = ClosingCancel;
        ClosingRaised++;  
    } 

    public Task OnClosingAsync(CancelEventArgs e)
    {
        e.Cancel = ClosingAsyncCancel;
        ClosingAsyncRaised++;
        return Task.CompletedTask;  
    } 

    public void OnClosed() => ClosedRaised++;
}
