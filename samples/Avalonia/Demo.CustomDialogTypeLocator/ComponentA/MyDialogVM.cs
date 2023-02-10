using ReactiveUI;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Avalonia.CustomDialogTypeLocator.ComponentA;

public class MyDialogVm : ViewModelBase, IModalDialogViewModel
{
    private bool? _dialogResult;

    public bool? DialogResult
    {
        get => _dialogResult;
        private set => this.RaiseAndSetIfChanged(ref _dialogResult, value);
    }
}
