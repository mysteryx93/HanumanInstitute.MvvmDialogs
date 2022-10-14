using Avalonia.Markup.Xaml;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Navigation;

/// <summary>
/// The default application main window for single-view applications on desktop. 
/// </summary>
public partial class NavigationRootWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the NavigationRootWindow control.
    /// </summary>
    public NavigationRootWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

