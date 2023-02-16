using System.Threading.Tasks;
using System.Windows.Input;
using FluentAvalonia.UI.Controls;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia.Fluent;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using ReactiveUI;

namespace Demo.Avalonia.FluentContentDialog;

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
        ContentDialogSettings settings = new()
        {
            Content = "This is the text.",
            Title = "This Is The Caption",
            PrimaryButtonText = "OK",
            SecondaryButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Secondary
        };
        var result = await _dialogService.ShowContentDialogAsync(this, settings);

        UpdateResult(result == ContentDialogResult.Primary);
    }

    private void UpdateResult(bool? result) =>
        Confirmation = result == true ? "We got confirmation to continue!" : string.Empty;
}
