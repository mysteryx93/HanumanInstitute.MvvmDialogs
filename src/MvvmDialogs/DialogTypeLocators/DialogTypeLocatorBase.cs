using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HanumanInstitute.MvvmDialogs.DialogTypeLocators;

/// <summary>
/// Dialog type locator responsible for locating dialog types for specified view models based
/// on a naming convention used in a multitude of articles and code samples regarding the MVVM
/// pattern.
/// <para/>
/// The convention states that if the name of the view model is
/// 'MyNamespace.ViewModels.MyDialogViewModel' then the name of the dialog is
/// 'MyNamespace.Views.MyDialog'.
/// </summary>
public abstract class DialogTypeLocatorBase : IDialogTypeLocator
{
    /// <summary>
    /// Internal cache.
    /// </summary>
    internal static readonly DialogTypeLocatorCache Cache = new DialogTypeLocatorCache();

    /// <inheritdoc />
    public Type Locate(INotifyPropertyChanged viewModel)
    {
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        var viewModelType = viewModel.GetType();
        var dialogType = Cache.Get(viewModelType);
        if (dialogType != null)
        {
            return dialogType;
        }

        var locations = LocateViewNames(viewModelType.FullName!);
        foreach (var item in locations)
        {
            dialogType = viewModelType.Assembly.GetType(item);
            if (dialogType != null)
            {
                break;
            }
        }

        if (dialogType == null)
        {
            var errLocations = locations.Count() > 1 ? string.Join("' or '", locations.AsEnumerable()) : $"'{locations.FirstOrDefault()}'";
            throw new TypeLoadException(AppendInfoAboutDialogTypeLocators($"Dialog with name {errLocations} is missing."));
        }

        Cache.Add(viewModelType, dialogType);

        return dialogType;
    }

    /// <summary>
    /// Returns the view names to search to match specified ViewModel.
    /// </summary>
    /// <param name="viewModelName">The ViewModel to find the View for.</param>
    /// <returns>A list of possible matching paths.</returns>
    protected abstract IList<string> LocateViewNames(string viewModelName);

    private static string AppendInfoAboutDialogTypeLocators(string errorMessage) =>
        errorMessage + Environment.NewLine +
        "If your project structure doesn't conform to the default convention of MVVM " +
        "Dialogs you can always define a new convention by implementing your own dialog " +
        "type locator. For more information on how to do that, please read the GitHub " +
        "wiki or ask the author.";
}
