
namespace HanumanInstitute.MvvmDialogs.Wpf.Api;

internal class MessageBoxApiSettings
{
    public string MessageBoxText { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public System.Windows.MessageBoxButton Buttons { get; set; }
    public System.Windows.MessageBoxImage Icon { get; set; }
    public System.Windows.MessageBoxResult DefaultButton { get; set; }
    public System.Windows.MessageBoxOptions Options { get; set; }
}
