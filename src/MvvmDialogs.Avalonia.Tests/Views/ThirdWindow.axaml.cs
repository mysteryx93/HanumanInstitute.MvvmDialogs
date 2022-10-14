using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests.Views;

public partial class ThirdWindow : Window
{
    public ThirdWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

