using System;
using System.ComponentModel;
using System.Linq;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Xunit;
using Moq;

namespace Demo.Avalonia.ModalDialog.Tests;

public class MainWindowViewModelTests
{
    public MainWindowViewModel Model => _model ??= new MainWindowViewModel(DialogService);
    private MainWindowViewModel? _model;

    public DialogService DialogService => _dialogService ??= new DialogService(MockDialogManager.Object, viewModelFactory: ViewModelFactory);
    private DialogService? _dialogService;

    public Mock<IDialogManager> MockDialogManager => _mockDialogManager ??= new Mock<IDialogManager>();
    private Mock<IDialogManager>? _mockDialogManager;

    private object? ViewModelFactory(Type type) => (type) switch
    {
        _ when type == typeof(AddTextDialogViewModel) => new AddTextDialogViewModel(),
        _ => null
    };

    [Fact]
    public void ShowDialogCommand_InputValue_AddToList()
    {
        var newText = "abc";
        MockDialogManager.Setup(x => x.ShowDialogAsync(It.IsAny<INotifyPropertyChanged>(), It.IsAny<AddTextDialogViewModel>()))
            .Callback<INotifyPropertyChanged, IModalDialogViewModel>(
                (ownerViewModel, viewModel) =>
                {
                    var vm = (AddTextDialogViewModel)viewModel;
                    vm.Text = newText;
                    vm.DialogResult = true;
                });

        Model.ImplicitShowDialogCommand.Execute(null);

        Assert.Single(Model.Texts);
        Assert.Equal(newText, Model.Texts.First());
    }
}
