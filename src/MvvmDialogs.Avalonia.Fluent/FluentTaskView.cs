using System.ComponentModel;
using HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Fluent;

/// <summary>
/// An <see cref="IView"/> implementation for FluentContentDialog.
/// </summary>
public class FluentTaskView : IView
{
    /// <summary>
    /// Initializes a new instance of the FluentTaskView.
    /// </summary>
    /// <param name="settings">The TaskDialog display settings.</param>
    public FluentTaskView(TaskDialogSettings settings)
    {
        Settings = settings;
    }

    /// <summary>
    /// Gets or sets the display settings.
    /// </summary>
    public TaskDialogSettings Settings { get; set; }

    /// <summary>
    /// Gets or sets the dialog result.
    /// </summary>
    public object DialogResult { get; set; }
    
    /// <inheritdoc />
    public void Initialize(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
    { }

    /// <inheritdoc />
    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    { }

    /// <inheritdoc />
    public INotifyPropertyChanged? ViewModel => Settings.Content as INotifyPropertyChanged;
    /// <summary>
    /// Gets a reference to the displayed TaskDialog.
    /// </summary>
    public TaskDialog? Ref { get; private set; }
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
        var dialog = new TaskDialog()
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
        if (owner.GetRef() != null)
        {
            dialog.XamlRoot = TopLevel.GetTopLevel(owner.GetRef());
        }
        
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
