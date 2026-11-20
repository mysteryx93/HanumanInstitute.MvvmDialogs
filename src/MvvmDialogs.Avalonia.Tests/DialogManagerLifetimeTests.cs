namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

public class DialogManagerLifetimeTests
{
    [Fact]
    public void HandleDialogEvents_RequestClose_ClosesDialog()
    {
        var manager = CreateManager();
        var viewModel = new FirstViewModel();
        var dialog = new FakeView();

        manager.HandleDialogEvents(viewModel, dialog);
        viewModel.OnRequestClose();

        Assert.Equal(1, dialog.CloseCount);
    }

    [Fact]
    public void HandleDialogEvents_AfterClosed_RequestCloseDoesNotRetainDialog()
    {
        var manager = CreateManager();
        var viewModel = new FirstViewModel();
        var dialog = new FakeView();

        manager.HandleDialogEvents(viewModel, dialog);
        dialog.RaiseClosed();
        viewModel.OnRequestClose();

        Assert.Equal(0, dialog.CloseCount);
    }

    [Fact]
    public void HandleDialogEvents_AfterClosed_RequestActivateDoesNotRetainDialog()
    {
        var manager = CreateManager();
        var viewModel = new FirstViewModel();
        var dialog = new FakeView();

        manager.HandleDialogEvents(viewModel, dialog);
        dialog.RaiseClosed();
        viewModel.OnRequestActivate();

        Assert.Equal(0, dialog.ActivateCount);
    }

    [Fact]
    public void HandleDialogEvents_ClosedCallbackThrows_DetachesViewModelRequests()
    {
        var manager = CreateManager();
        var viewModel = new ThrowingCloseViewModel();
        var dialog = new FakeView();
        manager.HandleDialogEvents(viewModel, dialog);

        var close = dialog.RaiseClosed;

        Assert.Throws<InvalidOperationException>(close);
        viewModel.OnRequestClose();
        viewModel.OnRequestActivate();
        Assert.Equal(0, dialog.CloseCount);
        Assert.Equal(0, dialog.ActivateCount);
    }

    [Fact]
    public async Task Close_NativeWindowSubscriberThrows_StillNotifiesViewModel()
    {
        await using var ui = new HeadlessSession();
        await ui.Run(() =>
        {
            var manager = new DialogManager();
            var model = new FirstViewModel();
            var view = new ViewWrapper();
            view.InitializeExisting(model, new Window());
            var failure = new InvalidOperationException("Close subscriber failed.");
            Exception reported = null;
            view.Closed += (_, _) => throw failure;
            using var events = manager.HandleDialogEvents(model, view);
            Dispatcher.UIThread.UnhandledException += OnUnhandled;
            try
            {
                view.Show(null);

                view.Close();
                Dispatcher.UIThread.RunJobs();

                Assert.Same(failure, reported);
                Assert.Equal(1, model.ClosedCount);
            }
            finally { Dispatcher.UIThread.UnhandledException -= OnUnhandled; }

            void OnUnhandled(object sender, DispatcherUnhandledExceptionEventArgs e)
            {
                reported = e.Exception;
                e.Handled = true;
            }
        });
    }

    private class ThrowingCloseViewModel : FirstViewModel
    {
        public override void OnClosed() => throw new InvalidOperationException("Close callback failed.");
    }

    [Fact]
    public void HandleDialogEvents_Closed_DoesNotKeepDialogAliveFromViewModel()
    {
        var manager = CreateManager();
        var viewModel = new FirstViewModel();
        var dialogRef = SubscribeAndClose(manager, viewModel);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.False(dialogRef.IsAlive);
        // Keep the view model rooted so a surviving RequestClose handler would pin the dialog.
        GC.KeepAlive(viewModel);
        GC.KeepAlive(manager);
    }

    [Fact]
    public void HandleDialogEvents_WhenDisposed_RequestCloseDoesNotCloseDialog()
    {
        var manager = CreateManager();
        var viewModel = new FirstViewModel();
        var dialog = new FakeView();

        var events = manager.HandleDialogEvents(viewModel, dialog);
        events.Dispose();
        viewModel.OnRequestClose();

        Assert.Equal(0, dialog.CloseCount);
    }

    [Fact]
    public void HandleDialogEvents_WhenDisposed_DoesNotKeepDialogAliveFromViewModel()
    {
        var manager = CreateManager();
        var viewModel = new FirstViewModel();
        var dialogRef = SubscribeAndDispose(manager, viewModel);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.False(dialogRef.IsAlive);
        GC.KeepAlive(viewModel);
        GC.KeepAlive(manager);
    }

    [Fact]
    public void Show_WhenShowThrows_RequestCloseDoesNotCloseDialog()
    {
        var dialog = new FakeView
        {
            ShowException = new InvalidOperationException("show failed")
        };
        var manager = CreateManager();
        manager.CreatedDialog = dialog;
        var viewModel = new FirstViewModel();

        var ex = Assert.Throws<InvalidOperationException>(() => manager.Show(null, viewModel));

        Assert.Equal("show failed", ex.Message);
        viewModel.OnRequestClose();
        Assert.Equal(0, dialog.CloseCount);
    }

    [Fact]
    public void Show_WhenShowThrows_DoesNotKeepDialogAliveFromViewModel()
    {
        var viewModel = new FirstViewModel();
        var manager = CreateManager();
        var dialogRef = ShowThrowingDialog(manager, viewModel);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.False(dialogRef.IsAlive);
        GC.KeepAlive(viewModel);
        GC.KeepAlive(manager);
    }

    [Fact]
    public async Task ShowDialogAsync_WhenShowThrows_RequestCloseDoesNotCloseDialog()
    {
        var dialog = new FakeView
        {
            ShowException = new InvalidOperationException("show failed")
        };
        var manager = CreateManager();
        manager.CreatedDialog = dialog;
        manager.OwnerView = new FakeView();
        var viewModel = new SecondViewModel();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => manager.ShowDialogAsync(new FirstViewModel(), viewModel));

        Assert.Equal("show failed", ex.Message);
        viewModel.OnRequestClose();
        Assert.Equal(0, dialog.CloseCount);
    }

    [Fact]
    public void HandleDialogEvents_ReusedViewModel_OnlyCurrentDialogReceivesClose()
    {
        var manager = CreateManager();
        var viewModel = new FirstViewModel();
        var first = new FakeView();
        var second = new FakeView();

        manager.HandleDialogEvents(viewModel, first);
        first.RaiseClosed();
        manager.HandleDialogEvents(viewModel, second);
        viewModel.OnRequestClose();

        Assert.Equal(0, first.CloseCount);
        Assert.Equal(1, second.CloseCount);
    }

    [Fact]
    public async Task ShowFrameworkDialogAsync_WhenFactoryThrows_ClosesDummyOwner()
    {
        var factory = new Mock<IDialogFactory>();
        factory
            .Setup(x => x.ShowDialogAsync(It.IsAny<IView>(), It.IsAny<MessageBoxSettings>()))
            .ThrowsAsync(new InvalidOperationException("dialog failed"));
        var dummy = new FakeView();
        var manager = new FakeDialogManager(factory.Object)
        {
            DummyWindow = dummy
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => manager.ShowFrameworkDialogAsync(null, new MessageBoxSettings()));

        Assert.Equal(1, dummy.CloseCount);
    }

    [Fact]
    public async Task ShowFrameworkDialogAsync_WhenFactoryCompletes_ClosesDummyOwner()
    {
        var factory = new Mock<IDialogFactory>();
        factory
            .Setup(x => x.ShowDialogAsync(It.IsAny<IView>(), It.IsAny<MessageBoxSettings>()))
            .ReturnsAsync("ok");
        var dummy = new FakeView();
        var manager = new FakeDialogManager(factory.Object)
        {
            DummyWindow = dummy
        };

        var result = await manager.ShowFrameworkDialogAsync(null, new MessageBoxSettings());

        Assert.Equal("ok", result);
        Assert.Equal(1, dummy.CloseCount);
    }

    [Fact]
    public void HandleDialogEvents_NestedClose_AllowsOtherView()
    {
        var manager = CreateManager();
        var parentVm = new ClosingProbe();
        var childVm = new ClosingProbe();
        var parent = new FakeView
        {
            ViewModel = parentVm
        };
        var child = new FakeView
        {
            ViewModel = childVm
        };
        var parentArgs = new CancelEventArgs();
        var childArgs = new CancelEventArgs();
        parentVm.OnClosingAction = _ => child.RaiseClosing(childArgs);
        using var parentEvents = manager.HandleDialogEvents(parentVm, parent);
        using var childEvents = manager.HandleDialogEvents(childVm, child);

        parent.RaiseClosing(parentArgs);

        Assert.False(parentArgs.Cancel);
        Assert.False(childArgs.Cancel);
        Assert.True(child.ClosingConfirmed);
    }

    [Fact]
    public async Task ShowFrameworkDialogAsync_AllowConcurrentChanges_NextDialogCompletes()
    {
        var factory = new Mock<IDialogFactory>();
        var dummy = new FakeView();
        var manager = new FakeDialogManager(factory.Object)
        {
            DummyWindow = dummy
        };
        factory
            .Setup(x => x.ShowDialogAsync(It.IsAny<IView>(), It.IsAny<MessageBoxSettings>()))
            .Returns(() =>
            {
                manager.AllowConcurrentDialogs = true;
                return Task.FromResult<object>("ok");
            });

        await manager.ShowFrameworkDialogAsync(null, new MessageBoxSettings());
        manager.AllowConcurrentDialogs = false;
        var second = manager.ShowFrameworkDialogAsync(null, new MessageBoxSettings());
        var done = await Task.WhenAny(second, Task.Delay(1000, TestContext.Current.CancellationToken));

        Assert.Same(second, done);
        Assert.Equal("ok", await second);
    }

    private static FakeDialogManager CreateManager() =>
        new(new Mock<IDialogFactory>().Object);

    private sealed class ClosingProbe : INotifyPropertyChanged, IViewClosing
    {
        public event PropertyChangedEventHandler PropertyChanged { add { } remove { } }

        public Action<CancelEventArgs> OnClosingAction { get; set; }

        public void OnClosing(CancelEventArgs e) => OnClosingAction?.Invoke(e);

        public Task OnClosingAsync(CancelEventArgs e) => Task.CompletedTask;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference SubscribeAndClose(FakeDialogManager manager, FirstViewModel viewModel)
    {
        var dialog = new FakeView();
        var dialogRef = new WeakReference(dialog);
        manager.HandleDialogEvents(viewModel, dialog);
        dialog.RaiseClosed();
        return dialogRef;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference SubscribeAndDispose(FakeDialogManager manager, FirstViewModel viewModel)
    {
        var dialog = new FakeView();
        var dialogRef = new WeakReference(dialog);
        manager.HandleDialogEvents(viewModel, dialog).Dispose();
        return dialogRef;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference ShowThrowingDialog(FakeDialogManager manager, FirstViewModel viewModel)
    {
        var dialog = new FakeView
        {
            ShowException = new InvalidOperationException("show failed")
        };
        var dialogRef = new WeakReference(dialog);
        manager.CreatedDialog = dialog;
        try
        {
            manager.Show(null, viewModel);
        }
        catch (InvalidOperationException)
        {
            // expected
        }
        finally
        {
            manager.CreatedDialog = null;
        }

        return dialogRef;
    }
}
