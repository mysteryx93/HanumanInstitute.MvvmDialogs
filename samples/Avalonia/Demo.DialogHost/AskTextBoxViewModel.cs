using System;
using System.Reactive;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.SourceGenerators;

namespace Demo.Avalonia.DialogHost;

public partial class AskTextBoxViewModel : ViewModelBase, IModalDialogViewModel, ICloseable
{
    public event EventHandler? RequestClose;
    [ReactiveUI.SourceGenerators.Reactive]
    private bool? _dialogResult;

    [Reactive]
    public partial string Title { get; set; } = "Title";

    [Reactive]
    public partial string Text { get; set; } = string.Empty;

    public ReactiveCommand<RxVoid, RxVoid> Ok => _ok ??= ReactiveCommand.Create(OkImpl);
    private ReactiveCommand<RxVoid, RxVoid>? _ok;

    private void OkImpl()
    {
        DialogResult = true;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    public ReactiveCommand<RxVoid, RxVoid> Cancel => _cancel ??= ReactiveCommand.Create(CancelImpl);
    private ReactiveCommand<RxVoid, RxVoid>? _cancel;

    private void CancelImpl()
    {
        DialogResult = false;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
