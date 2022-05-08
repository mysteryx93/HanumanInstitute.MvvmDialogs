using System.Collections.Generic;

namespace HanumanInstitute.MvvmDialogs.DialogTypeLocators;

/// <summary>
/// Naming convention replacing 'ViewModel' with 'View' from folder path and suffix, and also trying without 'View' suffix.
/// For example: ViewModels/MainViewModel will return 'Views/MainView' and 'Views/Main'.
/// </summary>
public class NamingConventionDialogTypeLocator : DialogTypeLocatorBase
{
    /// <inheritdoc />
    protected override IList<string> LocateViewNames(string viewModelName)
    {
        var result = new List<string>();
        var dialogName = viewModelName.Replace("ViewModel", "View");
        result.Add(dialogName);
        if (dialogName.EndsWith("View"))
        {
            result.Add(dialogName.Substring(0, dialogName.Length - 4));
        }
        return result;
    }
}
