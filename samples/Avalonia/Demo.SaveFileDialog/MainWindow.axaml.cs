using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo.SaveFileDialog;

public partial class MainWindow : Window
{
    public MainWindow()
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
    //
    // protected override async void OnOpened(EventArgs e)
    // {
    //     var result = await ShowSaveFileAsync();
    //     base.OnOpened(e);
    // }
    //
    // private async Task<string?> ShowStrAsync() =>
    //     (string?)await ShowObjAsync();
    //
    // private async Task<object?> ShowObjAsync() =>
    //     await ShowSaveFileAsync();
    //
    // private async Task<string?> ShowSaveFileAsync()
    // {
    //     var dialog = new Avalonia.Controls.SaveFileDialog();
    //     return await dialog.ShowAsync(this).ConfigureAwait(true);
    // }
}
