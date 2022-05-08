using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Class responsible for writing log messages.
/// </summary>
public static class DialogLogger
{
    private static Action<string> writer = _ => { };

    /// <summary>
    /// Set this property to expose logs for diagnostics purposes.
    /// </summary>
    /// <example>
    /// This sample shows how messages are logged using <see cref="Debug.WriteLine(string)"/>.
    /// <code>
    /// Logger.Writer = message => Debug.WriteLine(message);
    /// </code>
    /// </example>
    public static Action<string> Writer
    {
        get => writer;
        set => writer = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Writes a log message into the writer.
    /// </summary>
    /// <param name="message">The log message to write.</param>
    /// <param name="callerFilePath">The file path of the caller.</param>
    /// <param name="callerMemberName">The member name of the caller.</param>
    public static void Write(
        string message,
        [CallerFilePath] string callerFilePath = "",
        [CallerMemberName] string callerMemberName = "")
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        Writer($"[{Path.GetFileNameWithoutExtension(callerFilePath)}.{callerMemberName}] {message}");
    }
}