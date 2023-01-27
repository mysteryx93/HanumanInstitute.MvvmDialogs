namespace HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

/// <summary>
/// Represents an item in the <see cref="ViewCache"/> 
/// </summary>
internal class ViewCacheItem
{
    public ViewCacheItem(Type viewModelType, Type viewType, UserControl view)
    {
        ViewModelType = viewModelType;
        ViewType = viewType;
        View = new WeakReference<UserControl>(view);
    }
    
    /// <summary>
    /// The data type of the ViewModel.
    /// </summary>
    public Type ViewModelType { get; }
    /// <summary>
    /// The data type of the View associated with the ViewModel.
    /// </summary>
    public Type ViewType { get; set; }
    /// <summary>
    /// A weak reference to a View instance.
    /// </summary>
    // public WeakReference<UserControl> View { get; set; }
    public WeakReference<UserControl> View { get; set; }
}
