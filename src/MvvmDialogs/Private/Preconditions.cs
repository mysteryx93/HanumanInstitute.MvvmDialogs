using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace HanumanInstitute.MvvmDialogs.Private;

internal static class Preconditions
{
    // Validates and returns the argument, following HanumanInstitute.Validators.
    [return: NotNull]
    internal static T CheckNotNull<T>([NotNull] this T value, [CallerArgumentExpression(nameof(value))] string name = "") =>
        value is null ? throw new ArgumentNullException(name) : value;
}
