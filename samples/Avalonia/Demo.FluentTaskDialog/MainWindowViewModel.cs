using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentAvalonia.UI.Controls;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia.Fluent;
using ReactiveUI;

namespace Demo.Avalonia.FluentTaskDialog;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    public ICommand ShowMessageBoxWithMessageCommand { get; }

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        ShowMessageBoxWithMessageCommand = ReactiveCommand.CreateFromTask(ShowMessageBoxWithMessage);
    }

    private string _confirmation = string.Empty;
    public string Confirmation
    {
        get => _confirmation;
        private set => this.RaiseAndSetIfChanged(ref _confirmation, value, nameof(Confirmation));
    }

    private async Task ShowMessageBoxWithMessage()
    {
        var settings = new TaskDialogSettings()
        {
            Title = "Task Dialog Title",
            Content = "Click whatever you want",
            Buttons = new List<TaskDialogButton>()
            {
                TaskDialogButton.YesButton,
                TaskDialogButton.NoButton
            }
        };
        var result = await _dialogService.ShowTaskDialogAsync(this, settings);

        UpdateResult(result == TaskDialogStandardResult.Yes);
    }

    private void UpdateResult(bool? result) =>
        Confirmation = result == true ? "We got confirmation to continue!" : string.Empty;
}
