using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.Avalonia.DialogHost;

public partial class TextBoxView : UserControl
{
    public TextBoxView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

