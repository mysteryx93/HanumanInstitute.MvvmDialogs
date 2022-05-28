using System;
using System.Windows.Input;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;

namespace Demo.Logging;

public class AddTextDialogViewModel : ViewModelBase, IModalDialogViewModel, ICloseable
{
    private string text = string.Empty;
    private bool? dialogResult;
    public ICommand OkCommand { get; }
    public event EventHandler? RequestClose;

    public AddTextDialogViewModel()
    {
        OkCommand = ReactiveCommand.Create(Ok);
    }

    public string Text
    {
        get => text;
        set => this.RaiseAndSetIfChanged(ref text, value, nameof(Text));
    }

    public bool? DialogResult
    {
        get => dialogResult;
        private set => this.RaiseAndSetIfChanged(ref dialogResult, value, nameof(DialogResult));
    }

    private void Ok()
    {
        if (!string.IsNullOrEmpty(Text))
        {
            DialogResult = true;
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Cancel()
    {
        DialogResult = false;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
