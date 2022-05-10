using ReactiveUI;
using HanumanInstitute.MvvmDialogs;

namespace Demo.CustomDialogTypeLocator.ComponentA;

public class MyDialogVM : ViewModelBase, IModalDialogViewModel
{
    private bool? dialogResult;

    public bool? DialogResult
    {
        get => dialogResult;
        private set => this.RaiseAndSetIfChanged(ref dialogResult, value);
    }
}
