using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.MessageBox;

namespace Demo.CustomMessageBox;

public class CustomFrameworkDialogFactory : DefaultFrameworkDialogFactory
{
    public override IMessageBox CreateMessageBox(MessageBoxSettings settings)
    {
        return new CustomMessageBox(settings);
    }
}
