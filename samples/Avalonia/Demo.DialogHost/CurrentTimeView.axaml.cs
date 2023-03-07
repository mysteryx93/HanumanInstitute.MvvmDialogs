using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.Avalonia.DialogHost;

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
