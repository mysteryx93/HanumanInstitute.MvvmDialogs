using System;
using System.Reactive;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Demo.Avalonia.DialogHost;

public partial class AskTextBoxViewModel : ViewModelBase, IModalDialogViewModel, ICloseable
{
    public event EventHandler? RequestClose;
    public bool? DialogResult { get; set; }

    [Reactive]
    public partial string Title { get; set; } = "Title";

    [Reactive]
    public partial string Text { get; set; } = string.Empty;

    public ReactiveCommand<Unit, Unit> Ok => _ok ??= ReactiveCommand.Create(OkImpl);
    private ReactiveCommand<Unit, Unit>? _ok;

    private void OkImpl()
    {
        DialogResult = true;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    public ReactiveCommand<Unit, Unit> Cancel => _cancel ??= ReactiveCommand.Create(CancelImpl);
    private ReactiveCommand<Unit, Unit>? _cancel;

    private void CancelImpl()
    {
        DialogResult = false;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
