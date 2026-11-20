namespace HanumanInstitute.MvvmDialogs.Avalonia.Fluent;

/// <summary>
/// An <see cref="IView"/> implementation for FluentContentDialog.
/// </summary>
public class FluentContentView : IView
{
    /// <summary>
    /// Initializes a new instance of the FluentContentView.
    /// </summary>
    /// <param name="settings">The ContentDialog display settings.</param>
    public FluentContentView(ContentDialogSettings settings)
    {
        Settings = settings;
    }

    /// <summary>
    /// Gets or sets the display settings.
    /// </summary>
    public ContentDialogSettings Settings { get; set; }

    /// <summary>
    /// Gets or sets the dialog result.
    /// </summary>
    public FAContentDialogResult DialogResult { get; set; } = FAContentDialogResult.None;

    /// <inheritdoc />
    public void Initialize(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
    {
    }

    /// <inheritdoc />
    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    {
    }

    /// <inheritdoc />
    public INotifyPropertyChanged? ViewModel => Settings.Content as INotifyPropertyChanged;
    /// <summary>
    /// Gets a reference to the displayed ContentDialog.
    /// </summary>
    public FAContentDialog? Ref { get; private set; }
    /// <inheritdoc />
    public object RefObj => Ref!;
    /// <inheritdoc />
    public event EventHandler? Loaded;
    /// <inheritdoc />
    public event EventHandler<CancelEventArgs>? Closing;
    /// <inheritdoc />
    public event EventHandler? Closed;

    /// <inheritdoc />
    public void Show(IView? owner) => throw new NotImplementedException();

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">The owner is detached from its top level.</exception>
    public async Task ShowDialogAsync(IView owner)
    {
        var dialog = new FAContentDialog
        {
            Title = Settings.Title,
            Content = Settings.Content,
            CloseButtonText = Settings.CloseButtonText,
            PrimaryButtonText = Settings.PrimaryButtonText,
            SecondaryButtonText = Settings.SecondaryButtonText,
            DefaultButton = Settings.DefaultButton,
            IsPrimaryButtonEnabled = Settings.IsPrimaryButtonEnabled,
            IsSecondaryButtonEnabled = Settings.IsSecondaryButtonEnabled,
            FullSizeDesired = Settings.FullSizeDesired
        };
        Ref = dialog;

        var retiring = false;
        ExceptionDispatchInfo? loadFailure = null;
        ExceptionDispatchInfo? closeFailure = null;
        dialog.Loaded += (s, e) =>
        {
            try { Loaded?.Invoke(s, e); }
            catch (Exception error)
            {
                loadFailure = ExceptionDispatchInfo.Capture(error);
                Retire();
            }
        };
        dialog.Closing += (s, e) =>
        {
            if (retiring) { return; }

            var args = new CancelEventArgs();
            try { Closing?.Invoke(s, args); }
            finally { ClosingConfirmed = false; }
            if (args.Cancel)
            {
                e.Cancel = true;
            }
        };
        dialog.Closed += (s, e) =>
        {
            try { Closed?.Invoke(s, e); }
            catch (Exception error) { closeFailure = ExceptionDispatchInfo.Capture(error); }
        };

        try
        {
            DialogResult = await owner.RunOverlayAsync(host => dialog.ShowAsync(host), dialog.Hide, Retire);
        }
        finally
        {
            Ref = null;
        }
        if (loadFailure != null && closeFailure != null)
        {
            throw new AggregateException(loadFailure.SourceException, closeFailure.SourceException);
        }
        (loadFailure ?? closeFailure)?.Throw();

        void Retire()
        {
            if (retiring) { return; }
            retiring = true;
            Dispatcher.UIThread.Post(() =>
            {
                if (ReferenceEquals(Ref, dialog)) { dialog.Hide(); }
            });
        }
    }

    /// <inheritdoc />
    public void Activate()
    {
    }

    /// <inheritdoc />
    public void Close()
    {
        Ref?.Hide();
    }

    /// <inheritdoc />
    public bool IsEnabled { get; set; } = true;

    /// <inheritdoc />
    public bool IsVisible => Ref != null;

    /// <inheritdoc />
    public bool ClosingConfirmed { get; set; }
}
