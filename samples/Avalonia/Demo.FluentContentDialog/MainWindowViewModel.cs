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
    private readonly IDialogService dialogService;
    public ICommand ShowMessageBoxWithMessageCommand { get; }
    public ICommand ShowMessageBoxWithCaptionCommand { get; }
    public ICommand ShowMessageBoxWithButtonCommand { get; }
    public ICommand ShowMessageBoxWithIconCommand { get; }
    public ICommand ShowMessageBoxWithDefaultResultCommand { get; }

    public MainWindowViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        ShowMessageBoxWithMessageCommand = ReactiveCommand.CreateFromTask(ShowMessageBoxWithMessage);
        ShowMessageBoxWithCaptionCommand = ReactiveCommand.CreateFromTask(ShowMessageBoxWithCaption);
        ShowMessageBoxWithButtonCommand = ReactiveCommand.CreateFromTask(ShowMessageBoxWithButton);
        ShowMessageBoxWithIconCommand = ReactiveCommand.CreateFromTask(ShowMessageBoxWithIcon);
        ShowMessageBoxWithDefaultResultCommand = ReactiveCommand.CreateFromTask(ShowMessageBoxWithDefaultResult);
    }

    private string confirmation = string.Empty;
    public string Confirmation
    {
        get => confirmation;
        private set => this.RaiseAndSetIfChanged(ref confirmation, value, nameof(Confirmation));
    }

    private async Task ShowMessageBoxWithMessage()
    {
        var settings = new ContentDialogSettings()
        {
            Title = "Content Dialog Title",
            Content = "Click whatever you want",
            PrimaryButtonText = "Primary :)"
        };
        var result = await dialogService.ShowContentDialogAsync(this, settings);

        UpdateResult(result == ContentDialogResult.Primary);
    }

    private async Task ShowMessageBoxWithCaption()
    {
        var result = await dialogService.ShowMessageBoxAsync(
            this,
            "This is the text.",
            "This Is The Caption");

        UpdateResult(result);
    }

    private async Task ShowMessageBoxWithButton()
    {
        var result = await dialogService.ShowMessageBoxAsync(
            this,
            "This is the text.",
            "This Is The Caption",
            MessageBoxButton.OkCancel);

        UpdateResult(result);
    }

    private async Task ShowMessageBoxWithIcon()
    {
        var result = await dialogService.ShowMessageBoxAsync(
            this,
            "This is the text.",
            "This Is The Caption",
            MessageBoxButton.OkCancel,
            MessageBoxImage.Information);

        UpdateResult(result);
    }

    private async Task ShowMessageBoxWithDefaultResult()
    {
        var result = await dialogService.ShowMessageBoxAsync(
            this,
            "This is the text.",
            "This Is The Caption",
            MessageBoxButton.OkCancel,
            MessageBoxImage.Information,
            null);

        UpdateResult(result);
    }

    private void UpdateResult(bool? result) =>
        Confirmation = result == true ? "We got confirmation to continue!" : string.Empty;
}
