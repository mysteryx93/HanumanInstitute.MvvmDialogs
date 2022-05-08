using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;

namespace Demo.ModalCustomDialog;

public class AddTextCustomDialogViewModel : ObservableObject, IModalDialogViewModel, ICloseable
{
    public event EventHandler? RequestClose;
    private string? text;
    private bool? dialogResult;

    public AddTextCustomDialogViewModel()
    {
        OkCommand = new RelayCommand(Ok);
    }

    public string? Text
    {
        get => text;
        set => SetProperty(ref text, value);
    }

    public ICommand OkCommand { get; }

    public bool? DialogResult
    {
        get => dialogResult;
        private set => SetProperty(ref dialogResult, value);
    }

    private void Ok()
    {
        if (!string.IsNullOrEmpty(Text))
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
            DialogResult = true;
        }
    }
}
