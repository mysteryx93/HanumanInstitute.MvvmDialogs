using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.Avalonia.OpenFolderDialog;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
