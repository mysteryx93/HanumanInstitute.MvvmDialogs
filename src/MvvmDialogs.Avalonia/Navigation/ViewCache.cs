using System.Collections.Generic;
using System.Linq;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

/// <summary>
/// Cache of View instances with weak references.
/// </summary>
public class ViewCache
{
    private readonly IList<ViewCacheItem> _cache = new List<ViewCacheItem>();

    /// <summary>
    /// Returns a View for specified ViewModel type. It will only work if such a View has been created before.
    /// </summary>
    /// <param name="viewModel">The ViewModel associated with the View.</param>
    /// <returns>The View instance, or null.</returns>
    public UserControl? GetViewForViewModel(INotifyPropertyChanged viewModel)
    {
        var item = _cache.FirstOrDefault(x => x.ViewModelType == viewModel.GetType());
        if (item != null)
        {
            if (item.View.TryGetTarget(out var result))
            {
                result.DataContext = viewModel;
                return result;
            }
            else
            {
                var newView = (UserControl)item.ViewDef.Create();
                item.View = new WeakReference<UserControl>(newView);
                newView.DataContext = viewModel;
                return newView;
            }
        }
        return null;
    }

    
    /// <summary>
    /// Returns an instance of specified viewType. A single instance will be returned per type, and it will be cached with a weak reference. 
    /// </summary>
    /// <param name="viewModel">The view model associated with the view.</param>
    /// <param name="viewDef">The view definition including its type and how to create one.</param>
    /// <returns>The View instance.</returns>
    public UserControl GetView(INotifyPropertyChanged viewModel, ViewDefinition viewDef)
    {
        var item = _cache.FirstOrDefault(x => x.ViewModelType == viewModel.GetType());
        if (item is null)
        {
            item = new ViewCacheItem(viewModel.GetType(), viewDef, (UserControl)viewDef.Create());
            _cache.Add(item);
        }
        item.ViewDef = viewDef;
        
        if (item.View.TryGetTarget(out var result))
        {
            result.DataContext = viewModel;
            return result;
        }
        var newView = (UserControl)viewDef.Create();
        item.View = new WeakReference<UserControl>(newView);
        newView.DataContext = viewModel;
        return newView;
    }
}
