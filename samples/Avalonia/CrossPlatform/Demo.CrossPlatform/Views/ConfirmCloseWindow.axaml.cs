using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.CrossPlatform.Views;

public partial class ConfirmCloseWindow : Window
{
    public ConfirmCloseWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

