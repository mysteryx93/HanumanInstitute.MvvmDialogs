// ReSharper disable MemberCanBePrivate.Global

using System.ComponentModel;
using HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

/// <summary>
/// Provides unit tests for the <see cref="ViewNavigationWrapper" /> class.
/// </summary>
public class ViewNavigationWrapperTests
{
    public INotifyPropertyChanged ViewModel => _viewModel ??= new FirstViewModel();
    private INotifyPropertyChanged _viewModel;

    public ViewNavigationWrapper CreateView(bool existing)
    {
        var result = new ViewNavigationWrapper();
        if (existing)
        {
            result.InitializeExisting(ViewModel, new FirstView());
        }
        else
        {
            result.Initialize(ViewModel, typeof(FirstView));
        }
        result.SetNavigation(NavigationManager);
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
        var loadedRaised = false;
        view.Loaded += (_, _) => loadedRaised = true;
        
        view.Show(null);
        
        Assert.NotNull(view.Ref);
        Assert.True(view.IsEnabled);
        Assert.True(view.IsVisible);
        Assert.True(loadedRaised);
    }
    
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShowDialog_WithOwner_RefSetAndEnabledVisible(bool existing)
    {
        var owner = CreateView(existing);
        var view = CreateView(existing);
        var loadedRaised = false;
        view.Loaded += (_, _) => loadedRaised = true;

        owner.Show(null);
        _ = view.ShowDialogAsync(owner);

        Assert.NotNull(view.Ref);
        Assert.True(view.IsEnabled);
        Assert.True(view.IsVisible);
        Assert.True(loadedRaised);
    }
    
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Show_Close_ClosingAndClosedRaised(bool existing)
    {
        var view = CreateView(existing);
        var closingRaised = false;
        var closedRaised = false;
        view.Closing += (_, _) => closingRaised = true;
        view.Closed += (_, _) => closedRaised = true;
        
        view.Show(null);
        view.Close();
        
        Assert.True(closingRaised);
        Assert.True(closedRaised);
    }
    
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Activate_NotFound_DoNotShow(bool existing)
    {
        var view = CreateView(existing);
        var loadedRaised = false;
        view.Loaded += (_, _) => loadedRaised = true;
        
        view.Activate();

        Assert.Null(NavigationManager.CurrentView);
        Assert.False(loadedRaised);
    }
    
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Activate_FromHistory_ShowAndRaiseLoaded(bool existing)
    {
        var view1 = CreateView(existing);
        _viewModel = new FirstViewModel(); 
        var view2 = CreateView(existing);
        var loadedRaised = false;
        
        view1.Show(null);
        view2.Show(view1);
        view1.Loaded += (_, _) => loadedRaised = true;
        view1.Activate();

        Assert.Equal(view1.Ref, NavigationManager.CurrentView);
        Assert.True(loadedRaised);
    }
    
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Activate_AlreadyVisible_DoNotRaiseLoaded(bool existing)
    {
        var view = CreateView(existing);
        var loadedRaised = false;
        view.Show(null);
        view.Loaded += (_, _) => loadedRaised = true;
        
        view.Activate();

        Assert.Equal(view.Ref, NavigationManager.CurrentView);
        Assert.False(loadedRaised);
    }
}
