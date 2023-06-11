using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.Avalonia.FluentTaskDialog;

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

