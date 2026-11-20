namespace HanumanInstitute.MvvmDialogs.Avalonia.Fluent;

/// <summary>
/// An <see cref="IView"/> implementation for FluentContentDialog.
/// </summary>
/// <param name="settings">The TaskDialog display settings.</param>
public class FluentTaskView(TaskDialogSettings settings) : IView
{
    /// <summary>
    /// Gets or sets the display settings.
    /// </summary>
    public TaskDialogSettings Settings { get; set; } = settings;

    /// <summary>
    /// Gets or sets the dialog result.
    /// </summary>
    public object? DialogResult { get; set; }

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
    /// Gets a reference to the displayed TaskDialog.
    /// </summary>
    public FATaskDialog? Ref { get; private set; }
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
        var dialog = new FATaskDialog
        {
            Title = Settings.Title,
            Header = Settings.Header,
            SubHeader = Settings.SubHeader,
            Content = Settings.Content,
            IconSource = Settings.IconSource,
            FooterVisibility = Settings.FooterVisibility,
            IsFooterExpanded = Settings.IsFooterExpanded,
            Footer = Settings.Footer,
            ShowProgressBar = Settings.ShowProgressBar
        };
        Ref = dialog;
        foreach (var button in Settings.Buttons)
        {
            dialog.Buttons.Add(button);
        }

        var retiring = false;
        ExceptionDispatchInfo? loadFailure = null;
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
        dialog.Closed += (s, e) => Closed?.Invoke(s, e);

        try
        {
            DialogResult = await owner.RunOverlayAsync(
                host =>
                {
                    dialog.XamlRoot = host;
                    return dialog.ShowAsync();
                },
                dialog.Hide,
                Retire);
        }
        catch (Exception error) when (loadFailure != null)
        {
            throw new AggregateException(loadFailure.SourceException, error);
        }
        finally
        {
            Ref = null;
        }
        loadFailure?.Throw();

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
