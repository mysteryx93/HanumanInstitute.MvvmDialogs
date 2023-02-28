namespace HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

/// <summary>
/// Represents an item in the <see cref="ViewCache"/> 
/// </summary>
internal class ViewCacheItem
{
    public ViewCacheItem(Type viewModelType, ViewDefinition viewDef, UserControl view)
    {
        ViewModelType = viewModelType;
        ViewDef = viewDef;
        View = new WeakReference<UserControl>(view);
    }
    
    /// <summary>
    /// The data type of the ViewModel.
    /// </summary>
    public Type ViewModelType { get; }
    /// <summary>
    /// An action to create a view of desired type.
    /// </summary>
    public ViewDefinition ViewDef { get; set; }
    /// <summary>
    /// A weak reference to a View instance.
    /// </summary>
    public WeakReference<UserControl> View { get; set; }
}
