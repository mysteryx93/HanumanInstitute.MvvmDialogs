namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

/// <summary>
/// Minimal <see cref="IView"/> for lifetime and event tests.
/// </summary>
internal sealed class FakeView : IView
{
    public int CloseCount { get; private set; }
    public int ActivateCount { get; private set; }

    public void Initialize(INotifyPropertyChanged viewModel, ViewDefinition viewDef) => ViewModel = viewModel;

    public void InitializeExisting(INotifyPropertyChanged viewModel, object view) => ViewModel = viewModel;

    public object RefObj => this;

    public event EventHandler Loaded;
    public event EventHandler<CancelEventArgs> Closing;
    public event EventHandler Closed;

    public INotifyPropertyChanged ViewModel { get; set; }

    public Exception ShowException { get; set; }

    public void Show(IView owner)
    {
        if (ShowException != null)
        {
            throw ShowException;
        }
    }

    public Task ShowDialogAsync(IView owner)
    {
        if (ShowException != null)
        {
            throw ShowException;
        }

        return Task.CompletedTask;
    }

    public void Activate() => ActivateCount++;

    public void Close() => CloseCount++;

    public bool IsEnabled { get; set; } = true;

    public bool IsVisible { get; set; } = true;

    public bool ClosingConfirmed { get; set; }

    public void RaiseClosed() => Closed?.Invoke(this, EventArgs.Empty);

    public void RaiseLoaded() => Loaded?.Invoke(this, EventArgs.Empty);

    public void RaiseClosing(CancelEventArgs e) => Closing?.Invoke(this, e);
}
