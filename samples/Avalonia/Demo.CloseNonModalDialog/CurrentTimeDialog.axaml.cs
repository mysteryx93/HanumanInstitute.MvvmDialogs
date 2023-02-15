using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.Avalonia.CloseNonModalDialog;

public partial class CurrentTimeDialog : Window
{
    public CurrentTimeDialog()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
