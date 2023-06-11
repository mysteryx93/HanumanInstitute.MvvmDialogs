using System.ComponentModel;
using HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

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
    public ContentDialogResult DialogResult { get; set; } = ContentDialogResult.None;
    
    /// <inheritdoc />
    public void Initialize(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
    { }

    /// <inheritdoc />
    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    { }

    /// <inheritdoc />
    public INotifyPropertyChanged? ViewModel => Settings.Content as INotifyPropertyChanged;
    /// <summary>
    /// Gets a reference to the displayed ContentDialog.
    /// </summary>
    public ContentDialog? Ref { get; private set; }
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
    public async Task ShowDialogAsync(IView owner)
    {
        var dialog = new ContentDialog()
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

        dialog.Loaded += (s, e) => Loaded?.Invoke(s, e);
        dialog.Closing += (s, e) =>
        {
            var args = new CancelEventArgs();
            Closing?.Invoke(s, args);
            if (args.Cancel)
            {
                e.Cancel = true;
            }
        };
        dialog.Closed += (s, e) => Closed?.Invoke(s, e);

        // Allow the dialog to be closed by mobile back navigation.
        void Cancel() => dialog.Hide();
        CancellableActions.Add(Cancel);
        DialogResult = await dialog.ShowAsync().ConfigureAwait(true);
        CancellableActions.Remove(Cancel);
        Ref = null;
    }

    /// <inheritdoc />
    public void Activate()
    { }

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
