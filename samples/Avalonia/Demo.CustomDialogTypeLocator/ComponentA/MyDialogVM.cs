using ReactiveUI;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Avalonia.CustomDialogTypeLocator.ComponentA;

public class MyDialogVM : ViewModelBase, IModalDialogViewModel
{
    private bool? dialogResult;

    public bool? DialogResult
    {
        get => dialogResult;
        private set => this.RaiseAndSetIfChanged(ref dialogResult, value);
    }
}
