using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Xunit;
using Moq;

namespace Demo.ModalDialog.Tests;

public class MainWindowViewModelTests
{
    public MainWindowViewModel Model => model ??= new MainWindowViewModel(DialogService);
    private MainWindowViewModel? model;

    public DialogService DialogService => dialogService ??= new DialogService(MockDialogManager.Object, viewModelFactory: ViewModelFactory);
    private DialogService? dialogService;

    public Mock<IDialogManager> MockDialogManager => mockDialogManager ??= new Mock<IDialogManager>();
    private Mock<IDialogManager>? mockDialogManager;

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
