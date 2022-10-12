using Avalonia.Markup.Xaml;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// The default application main view for single-view applications. 
/// </summary>
public partial class NavigationRoot : UserControl
{
    /// <summary>
    /// Initializes a new instance of the NavigationRoot control.
    /// </summary>
    public NavigationRoot() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
