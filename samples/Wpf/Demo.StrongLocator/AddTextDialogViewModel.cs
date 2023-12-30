using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Wpf.StrongLocator;

public class AddTextDialogViewModel : ObservableObject, IModalDialogViewModel, ICloseable
{
    private string? _text;
    private bool? _dialogResult;

    public AddTextDialogViewModel()
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
        set => SetProperty(ref _dialogResult, value);
    }

    public event EventHandler? RequestClose;

    private void Ok()
    {
        if (!string.IsNullOrEmpty(Text))
        {
            DialogResult = true;
            this.RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
