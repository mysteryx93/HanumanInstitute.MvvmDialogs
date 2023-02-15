using System.Collections.Generic;
using System.Linq;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

/// <summary>
/// Provides a list of cancellable dialog actions such as messageboxes or popups. Useful for mobile back navigation.
/// This class is static (application-wide) because it would be difficult to connect the context between NavigationManager and the MessageBox API.  
/// </summary>
public static class CancellableActions
{
    private static readonly List<Action> s_list = new();

    /// <summary>
    /// Adds a cancellable dialog action to the list.
    /// </summary>
    /// <param name="action">An action to cancel the dialog.</param>
    public static void Add(Action action)
    {
        lock (s_list)
        {
            s_list.Add(action);
        }
    }

    /// <summary>
    /// Removes a cancellable action from the list. You must call this when the dialog is completed.
    /// </summary>
    /// <param name="action">The same action that was previously added.</param>
    public static void Remove(Action action)
    {
        lock (s_list)
        {
            s_list.Remove(action);
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
                return s_list.Any();           
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
    /// <returns>True if a dialog operation was cancelled; otherwise false.</returns>
    public static bool CancelLast()
    {
        lock (s_list)
        {
            if (s_list.Any())
            {
                s_list.Last().Invoke();
                s_list.RemoveAt(s_list.Count - 1);
                return true;
            }
        }
        return false;
    }
}
