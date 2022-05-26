using System.Threading;

namespace HanumanInstitute.MvvmDialogs.Private;

internal static class ViewIdGenerator
{
    private static int id;

    public static int Generate() => Interlocked.Increment(ref id);
}
