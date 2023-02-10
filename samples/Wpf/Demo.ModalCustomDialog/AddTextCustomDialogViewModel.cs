using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Wpf.ModalCustomDialog;

public class AddTextCustomDialogViewModel : ObservableObject, IModalDialogViewModel, ICloseable
{
    public event EventHandler? RequestClose;
    private string? _text;
    private bool? _dialogResult;

    public AddTextCustomDialogViewModel()
    {
        OkCommand = new RelayCommand(Ok);
    }

    public string? Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    public ICommand OkCommand { get; }

    public bool? DialogResult
    {
        get => _dialogResult;
        private set => SetProperty(ref _dialogResult, value);
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
