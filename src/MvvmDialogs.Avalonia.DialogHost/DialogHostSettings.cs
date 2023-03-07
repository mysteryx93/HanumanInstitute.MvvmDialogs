using System.ComponentModel;
using DialogHostAvalonia;

namespace HanumanInstitute.MvvmDialogs.Avalonia.DialogHost;

public class DialogHostSettings : DialogSettingsBase
{
    public INotifyPropertyChanged? ContentViewModel { get; set; }
    public DialogClosingEventHandler? ClosingHandler { get; set; }
    public bool CloseOnClickAway { get; set; }
    public object? CloseOnClickAwayParameter { get; set; }
}
