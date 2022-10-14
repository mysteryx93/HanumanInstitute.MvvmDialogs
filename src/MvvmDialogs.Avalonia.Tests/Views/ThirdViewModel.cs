using ReactiveUI;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests.Views;

public class ThirdViewModel : ReactiveObject, ICloseable, IActivable, IModalDialogViewModel
{
    public event EventHandler RequestClose;
    public event EventHandler RequestActivate;
    
    public void OnRequestClose() => RequestClose?.Invoke(this, EventArgs.Empty);

    public void OnRequestActivate() => RequestActivate?.Invoke(this, EventArgs.Empty);
    
    public bool? DialogResult { get; set; }
}
