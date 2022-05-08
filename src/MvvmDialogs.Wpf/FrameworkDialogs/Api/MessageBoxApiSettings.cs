using System.Windows;

namespace HanumanInstitute.MvvmDialogs.Wpf.FrameworkDialogs.Api;

internal class MessageBoxApiSettings
{
    public string MessageBoxText { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public MessageBoxButton Buttons { get; set; }
    public MessageBoxImage Icon { get; set; }
    public MessageBoxResult DefaultButton { get; set; }
    public MessageBoxOptions Options { get; set; }
}