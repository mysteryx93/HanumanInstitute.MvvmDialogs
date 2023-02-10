using System.Threading;

namespace HanumanInstitute.MvvmDialogs.Private;

internal static class ViewIdGenerator
{
    private static int s_id;

    public static int Generate() => Interlocked.Increment(ref s_id);
}
