using System.Linq;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using DialogHostAvalonia;

namespace HanumanInstitute.MvvmDialogs.Avalonia.DialogHost;

/// <summary>
/// An <see cref="IView"/> implementation for DialogHost.
/// </summary>
public class DialogHostView : IView
{
    private DialogSession? _session;

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
    {
    }

    /// <inheritdoc />
    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    {
    }

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

    /// <inheritdoc />
    public async Task ShowDialogAsync(IView owner)
    {
        var control = owner.GetOverlayOwner();
        var root = control is Window window ? window.Presenter : control;
        var host = (control as ContentControl)?.Content as DialogHostAvalonia.DialogHost ??
                   root?.FindDescendantOfType<DialogHostAvalonia.DialogHost>() ??
                   control?.GetLogicalDescendants().OfType<DialogHostAvalonia.DialogHost>().FirstOrDefault();
        try
        {
            if (host != null)
            {
                await owner.RunOverlayAsync(_ => ShowSessionAsync(host), Close, CloseHost);
                return;
            }

            await owner.WithOverlaySurfaceAsync(
                surface =>
                {
                    host = new();
                    if (Settings.Content != null && Settings.Content is not Control)
                    {
                        host.DialogContentTemplate = control?.FindDataTemplate(Settings.Content);
                    }
                    surface.Content = host;
                    return ShowSessionAsync(host);
                },
                Close,
                ownerClosed: CloseHost);
        }
        finally { _session = null; }

        // The native host closes every session without a veto when its owner disappears.
        void CloseHost() => host?.SetCurrentValue(DialogHostAvalonia.DialogHost.IsOpenProperty, false);
    }

    // Show one native session and forward the view-model events for that session.
    private async Task<object?> ShowSessionAsync(DialogHostAvalonia.DialogHost host)
    {
        host.CloseOnClickAway = Settings.CloseOnClickAway;
        host.CloseOnClickAwayParameter = Settings.CloseOnClickAwayParameter;
        host.PopupPositioner = Settings.PopupPositioner;
        host.OverlayBackground = Settings.OverlayBackground ?? host.OverlayBackground;
        host.DialogMargin = Settings.DialogMargin ?? host.DialogMargin;
        host.DisableOpeningAnimation = Settings.DisableOpeningAnimation;

        var loadFailed = false;
        var closingHandler = Settings.ClosingHandler;
        var pending = DialogHostAvalonia.DialogHost.Show(Settings.Content!, host, (_, e) => _session = e.Session, OnClosing);
        if (!pending.IsCompleted)
        {
            // Opening an additional session may leave IsOpen unchanged and skip the opened event.
            _session ??= host.CurrentSession;
            try { Loaded?.Invoke(this, EventArgs.Empty); }
            catch (Exception loadError)
            {
                loadFailed = true;
                try { Close(); }
                catch (Exception closeError) { throw new AggregateException(loadError, closeError); }
                finally
                {
                    // Force-closing the host retires every session if a native handler stranded this one.
                    if (_session != null && host.CurrentSessions.Contains(_session))
                    {
                        host.SetCurrentValue(DialogHostAvalonia.DialogHost.IsOpenProperty, false);
                    }
                }
                throw;
            }
        }
        DialogResult = await pending;
        Closed?.Invoke(this, EventArgs.Empty);
        return DialogResult;

        void OnClosing(object? sender, DialogClosingEventArgs e)
        {
            if (loadFailed || !e.CanBeCancelled) { return; }
            if (closingHandler != null)
            {
                closingHandler(sender!, e);
                return;
            }

            var args = new CancelEventArgs();
            try { Closing?.Invoke(this, args); }
            finally { ClosingConfirmed = false; }
            if (args.Cancel) { e.Cancel(); }
        }
    }

    /// <inheritdoc />
    public void Activate()
    {
    }

    /// <inheritdoc />
    public void Close()
    {
        // CloseDialogCommand throws when the session has already ended.
        var session = _session;
        if (session == null || session.IsEnded)
        {
            return;
        }

        session.Close();
    }

    /// <inheritdoc />
    public bool IsEnabled { get; set; } = true;

    /// <inheritdoc />
    public bool IsVisible => true;

    /// <inheritdoc />
    public bool ClosingConfirmed { get; set; }
}
