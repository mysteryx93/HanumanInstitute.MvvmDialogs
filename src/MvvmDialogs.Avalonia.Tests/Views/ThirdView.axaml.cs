using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests.Views;

public partial class ThirdView : UserControl
{
    public ThirdView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

