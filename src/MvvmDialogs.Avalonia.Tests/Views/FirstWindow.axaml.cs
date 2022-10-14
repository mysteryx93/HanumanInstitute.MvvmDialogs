using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests.Views;

public partial class FirstWindow : Window
{
    public FirstWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

