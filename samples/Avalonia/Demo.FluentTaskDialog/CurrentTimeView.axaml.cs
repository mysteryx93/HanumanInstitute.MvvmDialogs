using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.Avalonia.FluentTaskDialog;

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
