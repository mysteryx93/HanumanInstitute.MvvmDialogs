// ReSharper disable MemberCanBePrivate.Global

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

/// <summary>
/// Provides unit tests for the <see cref="ViewNavigationWrapper" /> class.
/// </summary>
public class ViewNavigationWrapperTests
{
    public FirstViewModel ViewModel => _viewModel ??= new();
    private FirstViewModel _viewModel;

    public ViewNavigationWrapper CreateView(bool existing)
    {
        var result = new ViewNavigationWrapper(NavigationManager, DialogManager.View_Closing);
        if (existing)
        {
            result.InitializeExisting(ViewModel, new FirstView());
        }
        else
        {
            result.Initialize(ViewModel, new(typeof(FirstView), () => new FirstView()));
        }
        return result;
    }

    public IDialogService DialogService => _dialogService ??= new DialogService(DialogManager);
    private IDialogService _dialogService;

    public DialogManager DialogManager => _dialogManager ??= new(
        new ViewLocatorBase
        {
            ForceSinglePageNavigation = true
        });
    private DialogManager _dialogManager;

    public INavigationManager NavigationManager => DialogManager.NavigationManager!;

    [Theory]
    [InlineData(null)]
    [InlineData("display text")]
    public void AsWrapper_UnsupportedDataContext_ReturnsNull(object dataContext)
    {
        var view = new FirstView
        {
            DataContext = dataContext
        };

        var wrapper = view.AsWrapper(NavigationManager, DialogManager.View_Closing);

        Assert.Null(wrapper);
    }

    [Fact]
    public void Show_CustomNavigationManager_UsesInterface()
    {
        var view = new FirstView();
        var navigation = new Mock<INavigationManager>();
        navigation.SetupGet(x => x.CurrentView).Returns(view);
        var definition = new ViewDefinition(typeof(FirstView), () => view);
        var wrapper = new ViewNavigationWrapper(navigation.Object, null);
        wrapper.Initialize(ViewModel, definition);

        wrapper.Show(null);

        navigation.Verify(x => x.Show(ViewModel, definition), Times.Once);
        Assert.Same(view, wrapper.RefObj);
    }

    [Fact]
    public async Task ShowDialogAsync_CustomNavigationManager_UsesInterface()
    {
        var view = new FirstView();
        var owner = CreateView(existing: true);
        var navigation = new Mock<INavigationManager>();
        navigation.SetupGet(x => x.CurrentView).Returns(view);
        var definition = new ViewDefinition(typeof(FirstView), () => view);
        var wrapper = new ViewNavigationWrapper(navigation.Object, null);
        wrapper.Initialize(ViewModel, definition);
        navigation.Setup(x => x.ShowDialogAsync(ViewModel, definition, owner.ViewModel)).Returns(Task.CompletedTask);

        await wrapper.ShowDialogAsync(owner);

        navigation.Verify(x => x.ShowDialogAsync(ViewModel, definition, owner.ViewModel), Times.Once);
        Assert.Same(view, wrapper.RefObj);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Initialize_ViewModelSet(bool existing)
    {
        var view = CreateView(existing);

        Assert.Equal(ViewModel, view.ViewModel);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Initialize_IsEnabledTrue(bool existing)
    {
        var view = CreateView(existing);

        Assert.True(view.IsEnabled);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Initialize_IsVisibleFalse(bool existing)
    {
        var view = CreateView(existing);

        Assert.False(view.IsVisible);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Initialize_RefDoesNotThrowError(bool existing)
    {
        var view = CreateView(existing);

        _ = view.Ref;
        _ = view.RefObj;
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Show_NoOwner_RefSetAndEnabledVisible(bool existing)
    {
        var view = CreateView(existing);

        var loaded = 0;
        view.Loaded += (_, _) => loaded++;
        view.Show(null);

        Assert.NotNull(view.Ref);
        Assert.True(view.IsEnabled);
        Assert.True(view.IsVisible);
        Assert.Equal(1, ViewModel.LoadedCount);
        Assert.Equal(1, loaded);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShowDialog_WithOwner_RefSetAndEnabledVisible(bool existing)
    {
        var owner = new ViewNavigationWrapper(NavigationManager, DialogManager.View_Closing);
        owner.Initialize(new FirstViewModel(), new(typeof(FirstView), () => new FirstView()));
        var view = CreateView(existing);

        owner.Show(null);
        ViewModel.ResetCounters();
        _ = view.ShowDialogAsync(owner);

        Assert.NotNull(view.Ref);
        Assert.True(view.IsEnabled);
        Assert.True(view.IsVisible);
        Assert.Equal(1, ViewModel.LoadedCount);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Show_Close_ClosingAndClosedRaised(bool existing)
    {
        var view = CreateView(existing);

        var closed = 0;
        view.Closed += (_, _) => closed++;
        view.Show(null);
        view.Close();

        Assert.Equal(1, ViewModel.ClosingCount);
        Assert.Equal(1, ViewModel.ClosedCount);
        Assert.Equal(1, closed);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Activate_NotFound_DoNotShow(bool existing)
    {
        var view = CreateView(existing);

        view.Activate();

        Assert.Null(NavigationManager.CurrentView);
        Assert.Equal(0, ViewModel.LoadedCount);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Activate_FromHistory_ShowAndRaiseLoaded(bool existing)
    {
        var vm1 = ViewModel;
        var view1 = CreateView(existing);
        var vm2 = new FirstViewModel();
        _viewModel = vm2;
        var view2 = CreateView(existing);

        view1.Show(null);
        view2.Show(view1);
        vm1.ResetCounters();
        view1.Activate();

        Assert.Equal(view1.Ref, NavigationManager.CurrentView);
        Assert.Equal(1, vm1.LoadedCount);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Activate_AlreadyVisible_DoNotRaiseLoaded(bool existing)
    {
        var view = CreateView(existing);
        view.Show(null);
        ViewModel.ResetCounters();

        view.Activate();

        Assert.Equal(view.Ref, NavigationManager.CurrentView);
        Assert.Equal(0, ViewModel.LoadedCount);
    }
}
