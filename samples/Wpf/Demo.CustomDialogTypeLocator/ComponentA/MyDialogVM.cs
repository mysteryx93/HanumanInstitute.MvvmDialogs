using CommunityToolkit.Mvvm.ComponentModel;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Wpf.CustomDialogTypeLocator.ComponentA;

public class MyDialogVm : ObservableObject, IModalDialogViewModel
{
    private bool? _dialogResult;

    public bool? DialogResult
    {
        get => _dialogResult;
        private set => SetProperty(ref _dialogResult, value);
    }
}
