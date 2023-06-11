using System.ComponentModel;
using Avalonia.VisualTree;
using HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

namespace HanumanInstitute.MvvmDialogs.Avalonia.DialogHost;

/// <summary>
/// An <see cref="IView"/> implementation for DialogHost.
/// </summary>
public class DialogHostView : IView
{
    private static readonly object s_lockCreateHost = new();

    /// <summary>
    /// Initializes a new instance of the DialogHostView.
    /// </summary>
    /// <param name="settings">The DialogHost display settings.</param>
    public DialogHostView(DialogHostSettings settings)
    {
        Settings = settings;
    }

    /// <summary>
    /// Gets or sets the display settings.
    /// </summary>
    public DialogHostSettings Settings { get; set; }

    /// <summary>
    /// Gets or sets the dialog result.
    /// </summary>
    public object? DialogResult { get; set; }

    /// <inheritdoc />
    public void Initialize(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
    { }

    /// <inheritdoc />
    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    { }

    /// <inheritdoc />
    public INotifyPropertyChanged? ViewModel => Settings.Content as INotifyPropertyChanged;
    /// <inheritdoc />
    public object RefObj => null!;
    /// <inheritdoc />
    public event EventHandler? Loaded;
    /// <inheritdoc />
    public event EventHandler<CancelEventArgs>? Closing;
    /// <inheritdoc />
    public event EventHandler? Closed;

    /// <inheritdoc />
    public void Show(IView? owner) => throw new NotImplementedException();

    /// <summary>
    /// Returns a DialogHost instance to manage the dialog.  
    /// </summary>
    private DialogHostAvalonia.DialogHost InitHost(ContentControl owner)
    {
        DialogHostAvalonia.DialogHost? host;
        lock (s_lockCreateHost) // lock to avoid creating host twice
        {
            host = owner.FindDescendantOfType<DialogHostAvalonia.DialogHost>();
            if (host == null)
            {
                host = new DialogHostAvalonia.DialogHost();
                var temp = owner.Content;
                owner.Content = null;
                host.Content = temp;
                owner.Content = host;
            }
        }
        Host = host;
        return host;
    }
    
    /// <summary>
    /// Returns the DialogHost that was last initiated.
    /// </summary>
    private DialogHostAvalonia.DialogHost? Host { get; set; }

    /// <inheritdoc />
    public async Task ShowDialogAsync(IView owner)
    {
        var host = InitHost(owner.GetRef()!);

        host.CloseOnClickAway = Settings.CloseOnClickAway;
        host.CloseOnClickAwayParameter = Settings.CloseOnClickAwayParameter;
        host.PopupPositioner = Settings.PopupPositioner;
        host.OverlayBackground = Settings.OverlayBackground ?? host.OverlayBackground;
        host.DialogMargin = Settings.DialogMargin ?? host.DialogMargin;
        host.DisableOpeningAnimation = Settings.DisableOpeningAnimation;

        var closingHandler = Settings.ClosingHandler ?? ((_, e) =>
        {
            var param = new CancelEventArgs();
            Closing?.Invoke(this, param);
            if (param.Cancel)
            {
                e.Cancel();
            }
        });

        try
        {
            host.DialogOpened += Host_DialogOpened;
            void Cancel() => Close();
            CancellableActions.Add(Cancel);
            DialogResult = await DialogHostAvalonia.DialogHost.Show(Settings.Content!, host, closingHandler).ConfigureAwait(true);
            CancellableActions.Remove(Cancel);
            Closed?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            host.DialogOpened -= Host_DialogOpened;
        }
    }

    private void Host_DialogOpened(object sender, EventArgs e)
    {
        Loaded?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void Activate()
    { }

    /// <inheritdoc />
    public void Close()
    {
        Host?.CloseDialogCommand.Execute(null);
    }

    /// <inheritdoc />
    public bool IsEnabled { get; set; } = true;

    /// <inheritdoc />
    public bool IsVisible => true;

    /// <inheritdoc />
    public bool ClosingConfirmed { get; set; }
}
