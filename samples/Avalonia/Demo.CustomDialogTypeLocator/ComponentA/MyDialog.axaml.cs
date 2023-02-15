using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.Avalonia.CustomDialogTypeLocator.ComponentA;

public partial class MyDialog : Window
{
    public MyDialog()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
