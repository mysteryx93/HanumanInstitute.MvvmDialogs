﻿using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Wpf.ViewEvents;

public class MainWindowViewModel : ObservableObject, IViewLoaded, IViewClosing, IViewClosed
{
    private readonly IDialogService _dialogService;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;
    }

    public string Text
    {
        get => _text;
        set => this.SetProperty(ref _text, value);
    }
    private string _text = string.Empty;

    public void OnClosed()
    {
        _dialogService.ShowMessageBox(null, "It's over.", "Closed");
    }

    public void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
    }

    public async Task OnClosingAsync(CancelEventArgs e)
    {
        var quit = await _dialogService.ShowMessageBoxAsync(this, "Do you really want to quit? ", "Confirmation", HanumanInstitute.MvvmDialogs.FrameworkDialogs.MessageBoxButton.YesNo);
        e.Cancel = quit != true;
    }

    public void OnLoaded()
    {
        Text = "Loaded!";
    }
}
