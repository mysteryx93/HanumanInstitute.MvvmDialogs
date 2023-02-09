using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.Avalonia.ModalDialog;

public partial class AddTextDialog : Window
{
    public AddTextDialog()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
