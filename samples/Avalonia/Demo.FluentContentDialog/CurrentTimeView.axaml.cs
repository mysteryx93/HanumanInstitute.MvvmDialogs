using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.Avalonia.FluentContentDialog;

public partial class CurrentTimeView : UserControl
{
    public CurrentTimeView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
