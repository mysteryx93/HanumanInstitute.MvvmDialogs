using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.Avalonia.FluentTaskDialog;

public partial class MessageView : UserControl
{
    public MessageView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

