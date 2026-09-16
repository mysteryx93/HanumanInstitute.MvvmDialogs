using Moq;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

/// <summary>
/// <see cref="DialogManagerBase{T}"/> with synchronous dispatch for unit tests.
/// </summary>
internal sealed class FakeDialogManager : DialogManagerBase<object>
{
    public FakeDialogManager(IDialogFactory dialogFactory, IViewLocator viewLocator = null)
        : base(viewLocator ?? CreateDefaultLocator(), dialogFactory, null)
    {
    }

    public IView DummyWindow { get; set; }
    public IView MainWindow { get; set; }
    public IView CreatedDialog { get; set; }
    public IView OwnerView { get; set; }

    protected override bool IsDesignMode => false;

    protected override IView CreateWrapper(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
    {
        if (CreatedDialog == null)
        {
            throw new NotSupportedException();
        }

        CreatedDialog.Initialize(viewModel, viewDef);
        return CreatedDialog;
    }

    protected override IView AsWrapper(object view) => throw new NotSupportedException();

    protected override void Dispatch(Action action) => action();

    protected override Task<D> DispatchAsync<D>(Func<D> action) => Task.FromResult(action());

    public override IView FindViewByViewModel(INotifyPropertyChanged viewModel) => OwnerView;

    private static IViewLocator CreateDefaultLocator()
    {
        var locator = new Mock<IViewLocator>();
        locator
            .Setup(x => x.Locate(It.IsAny<INotifyPropertyChanged>()))
            .Returns(new ViewDefinition(typeof(object), () => new object()));
        return locator.Object;
    }

    public override IView GetMainWindow() => MainWindow;

    public override IView GetDummyWindow() => DummyWindow;
}
