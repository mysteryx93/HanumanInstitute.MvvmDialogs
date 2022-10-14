// ReSharper disable MemberCanBePrivate.Global

using HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

public class NavigationTests
{
    public IDialogService DialogService => _dialogService ??= new DialogService(DialogManager);
    private IDialogService _dialogService;

    public DialogManager DialogManager => _dialogManager ??= new DialogManager(useSinglePageNavigation:true);
    private DialogManager _dialogManager;

    public INavigationManager NavigationManager => DialogManager.NavigationManager!;

    [Fact]
    public void Constructor_CurrentViewNull()
    {
        var _ = DialogService;

        Assert.Null(NavigationManager.CurrentView);
    }
    
    [Fact]
    public void Show_First_CurrentViewSet()
    {
        var vm = new FirstViewModel();
        
        DialogService.Show(null, vm);

        Assert.NotNull(NavigationManager.CurrentView);
        Assert.Equal(vm, NavigationManager.CurrentView.DataContext);
    }
    
    [Fact]
    public void Show_Second_CurrentViewSet()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();
        
        DialogService.Show(null, vm1);
        DialogService.Show(null, vm2);

        Assert.NotNull(NavigationManager.CurrentView);
        Assert.Equal(vm2, NavigationManager.CurrentView.DataContext);
    }
    
    [Fact]
    public void Show_FirstSecond_HistoryContainsFirstSecond()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();
        
        DialogService.Show(null, vm1);
        DialogService.Show(null, vm2);

        Assert.Equal(2, NavigationManager.History.Count);
        Assert.Same(vm1, NavigationManager.History[0]);
        Assert.Same(vm2, NavigationManager.History[1]);
    }
    
    [Fact]
    public void Show_FirstSecondFirst_HistoryContainsSecondFirst()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();
        
        DialogService.Show(null, vm1);
        DialogService.Show(null, vm2);
        DialogService.Show(null, vm1);

        Assert.Equal(2, NavigationManager.History.Count);
        Assert.Same(vm2, NavigationManager.History[0]);
        Assert.Same(vm1, NavigationManager.History[1]);
    }
    
    [Fact]
    public void Show_CloseSecond_CurrentViewSet()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();
        
        DialogService.Show(null, vm1);
        DialogService.Show(null, vm2);
        vm2.OnRequestClose();

        Assert.NotNull(NavigationManager.CurrentView);
        Assert.Equal(vm1, NavigationManager.CurrentView.DataContext);
    }
    
    [Fact]
    public void Show_CloseFirst_CloseApp()
    {
        var vm1 = new FirstViewModel();
        
        DialogService.Show(null, vm1);
        vm1.OnRequestClose();

        // ???
        Assert.False(true);
    }

    [Fact]
    public async Task Show_SecondAndGarbageCollect_FirstReleased()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();
        
        DialogService.Show(null, vm1);
        var view1 = new WeakReference(NavigationManager.CurrentView);
        DialogService.Show(null, vm2);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        await Task.Delay(100);
        Assert.False(view1.IsAlive);
    }
    
    
    [Fact]
    public void ShowDialogAsync_SecondAndGarbageCollect_FirstReleased()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();
        
        DialogService.Show(null, vm1);
        var view1 = new WeakReference(NavigationManager.CurrentView);
        var _ = DialogService.ShowDialogAsync(vm1, vm2);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Assert.False(view1.IsAlive);
    }
    
    [Fact]
    public void Activate_NotShown_DoNothing()
    {
        var vm1 = new FirstViewModel();
        
        vm1.OnRequestActivate();

        Assert.Null(NavigationManager.CurrentView);
    }
    
    [Fact]
    public void Activate_AlreadyActive_CurrentViewRemainsSame()
    {
        var vm1 = new FirstViewModel();
        
        DialogService.Show(null, vm1);
        vm1.OnRequestActivate();

        Assert.Same(vm1, NavigationManager.CurrentView?.DataContext);
        Assert.Single(NavigationManager.History);
    }
    
    [Fact]
    public void Activate_FirstSecondFirst_CurrentViewFirst()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();
        
        DialogService.Show(null, vm1);
        DialogService.Show(null, vm2);
        vm1.OnRequestActivate();

        Assert.Same(vm1, NavigationManager.CurrentView?.DataContext);
        Assert.Equal(2, NavigationManager.History.Count);
        Assert.Same(vm2, NavigationManager.History[0]);
        Assert.Same(vm1, NavigationManager.History[1]);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public async Task ShowDialogAsync_Second_ReturnsValue(bool? dialogResult)
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel
        {
            DialogResult = dialogResult
        };

        DialogService.Show(null, vm1);
        var task = DialogService.ShowDialogAsync(vm1, vm2);
        vm2.OnRequestClose();
        var result = await task;
        
        Assert.Equal(dialogResult, result);
    }
    
    [Fact]
    public async Task ShowDialogAsync_Second_WaitUntilClosed()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();

        DialogService.Show(null, vm1);
        var task = DialogService.ShowDialogAsync(vm1, vm2);
        
        await Task.Delay(10);
        Assert.False(task.IsCompleted);
        vm2.OnRequestClose();
        await Task.Delay(10);
        Assert.True(task.IsCompleted);
        Assert.Same(vm1, NavigationManager.CurrentView?.DataContext);
    }
    
    [Fact]
    public async Task ShowDialogAsync_RecursiveDialogs_WaitUntilClosed()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();
        var vm3 = new ThirdViewModel();

        DialogService.Show(null, vm1);
        var t1 = DialogService.ShowDialogAsync(vm1, vm2);
        var t2 = DialogService.ShowDialogAsync(vm2, vm3);
        
        await Task.Delay(10);
        Assert.False(t1.IsCompleted);
        Assert.False(t2.IsCompleted);
        vm3.OnRequestClose();
        vm2.OnRequestClose();
        await Task.Delay(10);
        Assert.True(t1.IsCompleted);
        Assert.True(t2.IsCompleted);
        Assert.Same(vm1, NavigationManager.CurrentView?.DataContext);
    }
    
    [Fact]
    public void ShowDialogAsync_Second_AddToHistory()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();

        DialogService.Show(null, vm1);
        var _ = DialogService.ShowDialogAsync(vm1, vm2);
        
        Assert.Equal(2, NavigationManager.History.Count);
        Assert.Same(vm1, NavigationManager.History[0]);
        Assert.Same(vm2, NavigationManager.History[1]);
    }
    
    [Fact]
    public async Task ShowDialogAsync_OwnerFirst_ShowFirstAfterClose()
    {
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();
        var vm3 = new ThirdViewModel();

        DialogService.Show(null, vm1);
        DialogService.Show(null, vm2);
        var task = DialogService.ShowDialogAsync(vm1, vm3);
        vm3.OnRequestClose();
        await task;

        Assert.Same(vm1, NavigationManager.CurrentView?.DataContext);
        Assert.Single(NavigationManager.History);
        Assert.Same(vm1, NavigationManager.History[0]);
    }
    
    [Fact]
    public void Show_TwiceWithinDialog_AddOneHistory()
    {
        // Only 1 view is stored in history under a dialog.
        var vm1 = new FirstViewModel();
        var vm2 = new SecondViewModel();
        var vm3 = new ThirdViewModel();
        var vm4 = new FirstViewModel();

        DialogService.Show(null, vm1);
        var _ = DialogService.ShowDialogAsync(vm1, vm2);
        DialogService.Show(null, vm3);
        DialogService.Show(null, vm4);

        Assert.Same(vm4, NavigationManager.CurrentView?.DataContext);
        Assert.Equal(3, NavigationManager.History.Count);
        Assert.Same(vm1, NavigationManager.History[0]);
        Assert.Same(vm2, NavigationManager.History[1]);
        Assert.Same(vm4, NavigationManager.History[2]);
    }
}
