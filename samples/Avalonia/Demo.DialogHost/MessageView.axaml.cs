using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.Avalonia.DialogHost;

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

