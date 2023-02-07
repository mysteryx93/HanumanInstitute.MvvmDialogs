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
                var newView = CreateView(item.ViewType);
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
    /// <param name="viewType">The type of the view displaying the view model.</param>
    /// <returns>The View instance.</returns>
    public UserControl GetView(INotifyPropertyChanged viewModel, Type viewType)
    {
        var item = _cache.FirstOrDefault(x => x.ViewModelType == viewModel.GetType());
        if (item is null)
        {
            item = new ViewCacheItem(viewModel.GetType(), viewType, CreateView(viewType));
            _cache.Add(item);
        }
        item.ViewType = viewType;
        
        if (item.View.TryGetTarget(out var result))
        {
            result.DataContext = viewModel;
            return result;
        }
        var newView = CreateView(viewType);
        item.View = new WeakReference<UserControl>(newView);
        newView.DataContext = viewModel;
        return newView;
    }

    private UserControl CreateView(Type viewType) => (UserControl)Activator.CreateInstance(viewType)!;
}
