
namespace HanumanInstitute.MvvmDialogs.Avalonia.DialogHost;

internal interface IDialogHostApi
{
    Task<object?> ShowDialogHostAsync(ContentControl? owner, DialogHostSettings settings);
}
