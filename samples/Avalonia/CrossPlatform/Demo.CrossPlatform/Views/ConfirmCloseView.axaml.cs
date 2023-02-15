using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.CrossPlatform.Views;

public partial class ConfirmCloseView : UserControl
{
    public ConfirmCloseView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

