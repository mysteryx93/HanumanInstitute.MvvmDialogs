using System.Collections.Generic;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

/// <summary>
/// Provides a list of cancellable dialog actions such as messageboxes or popups. Useful for mobile back navigation.
/// This class is static (application-wide) because it would be difficult to connect the context between NavigationManager and the MessageBox API.
/// </summary>
public static class CancellableActions
{
    private sealed class Registration(Action cancel)
    {
        public Action Cancel { get; } = cancel;
    }

    private static readonly List<Registration> s_list = [];
    
    /// <summary>
    /// Registers <paramref name="cancel"/> for back-navigation while <paramref name="action"/> runs,
    /// and always removes it when the action completes or throws.
    /// </summary>
    /// <param name="action">The dialog operation to run.</param>
    /// <param name="cancel">An action that cancels the dialog.</param>
    /// <typeparam name="T">The result type of the dialog operation.</typeparam>
    /// <returns>The result of <paramref name="action"/>.</returns>
    public static async Task<T> RunAsync<T>(Func<Task<T>> action, Action cancel)
    {
        var registration = new Registration(cancel);
        lock (s_list)
        {
            s_list.Add(registration);
        }
        try
        {
            return await action().ConfigureAwait(true);
        }
        finally
        {
            lock (s_list)
            {
                s_list.Remove(registration);
            }
        }
    }
    
    /// <summary>
    /// Returns whether there are active dialog actions.
    /// </summary>
    public static bool Any
    {
        get
        {
            lock (s_list)
            {
                return s_list.Count > 0;
            }
        }
    }

    /// <summary>
    /// Returns how many dialog actions are active.
    /// </summary>
    public static int Count
    {
        get
        {
            lock (s_list)
            {
                return s_list.Count;
            }
        }
    }

    /// <summary>
    /// Cancels the last dialog operation in the list.
    /// </summary>
    /// <returns>True if a dialog operation was canceled; otherwise false.</returns>
    public static bool CancelLast()
    {
        Action? action = null;
        lock (s_list)
        {
            if (s_list.Count > 0)
            {
                var last = s_list.Count - 1;
                action = s_list[last].Cancel;
                s_list.RemoveAt(last);
            }
        }
        action?.Invoke();
        return action != null;
    }
}
