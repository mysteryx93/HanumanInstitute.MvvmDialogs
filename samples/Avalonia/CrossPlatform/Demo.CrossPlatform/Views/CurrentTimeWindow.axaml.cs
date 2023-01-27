using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.CrossPlatform.Views;

public partial class CurrentTimeWindow : Window
{
    public CurrentTimeWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

