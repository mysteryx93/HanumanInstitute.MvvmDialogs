// ReSharper disable MemberCanBePrivate.Global

using HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

/// <summary>
/// Provides unit tests for the <see cref="ViewNavigationWrapper" /> class.
/// </summary>
public class ViewNavigationWrapperTests
{
    public FirstViewModel ViewModel => _viewModel ??= new FirstViewModel();
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
            result.Initialize(ViewModel, typeof(FirstView));
        }
        return result;
    }
    
    public IDialogService DialogService => _dialogService ??= new DialogService(DialogManager);
    private IDialogService _dialogService;

    public DialogManager DialogManager => _dialogManager ??= new DialogManager(new ViewLocatorBase() { ForceSinglePageNavigation = true });
    private DialogManager _dialogManager;

    public INavigationManager NavigationManager => DialogManager.NavigationManager!;

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
        
        view.Show(null);
        
        Assert.NotNull(view.Ref);
        Assert.True(view.IsEnabled);
        Assert.True(view.IsVisible);
        Assert.Equal(1, ViewModel.LoadedCount);
    }
    
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShowDialog_WithOwner_RefSetAndEnabledVisible(bool existing)
    {
        var owner = CreateView(existing);
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
        
        view.Show(null);
        view.Close();
        
        Assert.Equal(1, ViewModel.ClosingCount);
        Assert.Equal(1, ViewModel.ClosedCount);
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
