using Avalonia.Controls;
using MessageBox.Avalonia.Enums;

namespace HanumanInstitute.MvvmDialogs.Avalonia.MessageBox;

internal class MessageBoxApiSettings
{
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public ButtonEnum Buttons { get; set; } = ButtonEnum.Ok;
    public Icon Icon { get; set; } = Icon.None;
    public WindowStartupLocation StartupLocation { get; set; } = WindowStartupLocation.CenterScreen;
    public ClickEnum EnterDefaultButton { get; set; } = ClickEnum.Default;
    public ClickEnum EscDefaultButton { get; set; } = ClickEnum.Default;
}
