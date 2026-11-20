namespace HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

/// <summary>
/// Provides a list of cancellable dialog actions such as message boxes or popups, useful for mobile back navigation.
/// This class is static and application-wide so dialog integrations can register actions for navigation to dismiss.
/// Registrations can be scoped to an owner so Back dismisses a dialog in the requesting window.
/// </summary>
public static class CancellableActions
{
    private sealed class Registration(Action cancel, object? scope)
    {
        public Action Cancel { get; } = cancel;
        public object? Scope { get; } = scope;
        public bool Dismissing { get; set; }
    }

    private static readonly List<Registration> s_list = [];
    private static event EventHandler? Changed;
    internal static object RegistrationSource { get; } = new();
    internal static readonly WeakEvent<object, EventArgs> RegistrationsChanged = WeakEvent.Register<object>(
        static (_, handler) => Changed += handler,
        static (_, handler) => Changed -= handler);

    private static void NotifyChanged()
    {
        if (Changed == null) { return; }
        if (Dispatcher.UIThread.CheckAccess()) { Changed?.Invoke(RegistrationSource, EventArgs.Empty); }
        else { Dispatcher.UIThread.Post(NotifyChanged); }
    }

    /// <summary>
    /// Registers <paramref name="cancel"/> for back navigation while <paramref name="action"/> runs,
    /// and removes the registration when the action completes or throws.
    /// </summary>
    /// <param name="action">The dialog operation to run.</param>
    /// <param name="cancel">The action that dismisses the dialog.</param>
    /// <typeparam name="T">The result type of the dialog operation.</typeparam>
    /// <returns>The result of the dialog operation.</returns>
    public static Task<T> RunAsync<T>(Func<Task<T>> action, Action cancel) => RunAsync(action, cancel, null);

    /// <summary>
    /// Registers <paramref name="cancel"/> for back navigation while <paramref name="action"/> runs,
    /// and removes the registration when the action completes or throws.
    /// </summary>
    /// <param name="action">The dialog operation to run.</param>
    /// <param name="cancel">The action that dismisses the dialog.</param>
    /// <param name="scope">The dialog owner, or null for an application-wide registration.</param>
    /// <typeparam name="T">The result type of the dialog operation.</typeparam>
    /// <returns>The result of the dialog operation.</returns>
    public static async Task<T> RunAsync<T>(Func<Task<T>> action, Action cancel, object? scope)
    {
        var entry = new Registration(cancel, scope);
        lock (s_list) { s_list.Add(entry); }
        try
        {
            var pending = action();
            if (!pending.IsCompleted) { NotifyChanged(); }
            return await pending.ConfigureAwait(true);
        }
        finally
        {
            bool removed;
            lock (s_list) { removed = s_list.Remove(entry); }
            if (removed) { NotifyChanged(); }
        }
    }

    /// <summary>
    /// Returns whether there are active dialog actions.
    /// </summary>
    public static bool Any => Count != 0;

    /// <summary>
    /// Returns how many dialog actions are active.
    /// </summary>
    public static int Count
    {
        get
        {
            lock (s_list) { return s_list.Count; }
        }
    }

    /// <summary>
    /// Attempts to dismiss the last registered dialog.
    /// </summary>
    /// <returns>True if a matching dismissal was requested or is already in progress.</returns>
    public static bool CancelLast() => CancelLast(null);

    /// <summary>
    /// Attempts to dismiss the last dialog registered for this scope.
    /// </summary>
    /// <param name="scope">The owner or top-level view, or null to match any scope.</param>
    /// <returns>True if a matching dismissal was requested or is already in progress.</returns>
    public static bool CancelLast(object? scope) => CancelLast(scope, post: false);

    internal static bool CancelLast(object? scope, bool post)
    {
        Registration? entry;
        lock (s_list)
        {
            entry = LastMatching(scope);
            if (entry == null) { return false; }
            if (entry.Dismissing) { return true; }
            entry.Dismissing = true;
        }

        // Android needs dismissal to run after its platform Back event returns.
        if (post) { Dispatcher.UIThread.Post(Dismiss); }
        else { Dismiss(); }
        return true;

        void Dismiss()
        {
            lock (s_list)
            {
                if (!s_list.Contains(entry)) { return; }
            }
            try { entry.Cancel(); }
            finally
            {
                // A veto leaves the registration active for the next Back.
                lock (s_list) { entry.Dismissing = false; }
            }
        }
    }

    internal static bool CanCancel(object scope)
    {
        lock (s_list) { return LastMatching(scope) != null; }
    }

    private static Registration? LastMatching(object? scope) =>
        s_list.FindLast(entry => Matches(entry.Scope, scope));

    private static bool Matches(object? owner, object? request)
    {
        if (owner == null || request == null || ReferenceEquals(owner, request)) { return true; }
        if (owner is Visual visual && request is Visual source)
        {
            var top = TopLevel.GetTopLevel(visual);
            return top != null && ReferenceEquals(top, TopLevel.GetTopLevel(source));
        }
        return false;
    }
}
