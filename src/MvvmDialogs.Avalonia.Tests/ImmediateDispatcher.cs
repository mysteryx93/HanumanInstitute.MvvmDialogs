using Avalonia.Threading;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

public sealed class ImmediateDispatcher : IDispatcher
{
    public bool CheckAccess() => true;
    
    public void VerifyAccess() { }

    public void Post(Action action, DispatcherPriority priority = default) => action();
}
