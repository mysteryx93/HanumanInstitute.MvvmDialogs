using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.Avalonia.DialogHost;

public partial class AskTextBoxView : UserControl
{
    public AskTextBoxView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

