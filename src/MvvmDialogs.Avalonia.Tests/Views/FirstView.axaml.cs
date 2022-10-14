using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests.Views;

public partial class FirstView : UserControl
{
    public FirstView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

