using System.Collections.Generic;
using System.Linq;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Cache of View instances with weak references.
/// </summary>
public class ViewCache
{
    private readonly IList<ViewCacheItem> _cache = new List<ViewCacheItem>();

    /// <summary>
    /// Returns a View for specified ViewModel type. It will only work if such a View has been created before.
    /// </summary>
    /// <param name="viewModelType">The data type of the ViewModel associated with the View.</param>
    /// <returns>The View instance, or null.</returns>
    public UserControl? GetViewForViewModel(Type viewModelType)
    {
        var item = _cache.FirstOrDefault(x => x.ViewModelType == viewModelType);
        if (item != null)
        {
            if (item.View.TryGetTarget(out var result))
            {
                return result;
            }
        }
        return null;
    }

    
    /// <summary>
    /// Returns an instance of specified viewType. A single instance will be returned per type, and it will be cached with a weak reference. 
    /// </summary>
    /// <param name="viewModelType">The type of the view model associated with the view.</param>
    /// <param name="viewType">The type of the view displaying the view model.</param>
    /// <returns>The View instance.</returns>
    public UserControl GetView(Type viewModelType, Type viewType)
    {
        var item = _cache.FirstOrDefault(x => x.ViewModelType == viewModelType);
        if (item is null)
        {
            item = new ViewCacheItem(viewModelType, viewType, CreateView(viewType));
            _cache.Add(item);
        }
        item.ViewType = viewType;
        
        if (item.View.TryGetTarget(out var result))
        {
            return result;
        }
        var newView = CreateView(viewType);
        item.View = new WeakReference<UserControl>(newView);
        return newView;
    }

    private UserControl CreateView(Type viewType) => (UserControl)Activator.CreateInstance(viewType)!;
}
