using System.Collections.Generic;
using System.IO;

namespace HanumanInstitute.MvvmDialogs.PathInfo;

/// <summary>
/// Provides information about a directory path.
/// </summary>
public interface IDirectoryInfo : IPathInfo
{
    /// <summary>Returns an enumerable collection of file system information in the current directory.</summary>
    /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
    /// <returns>An enumerable collection of file system information in the current directory.</returns>
    IEnumerable<IPathInfo> EnumeratePathInfos();
    /// <summary>Returns an enumerable collection of file system information that matches a specified search pattern.</summary>
    /// <param name="searchPattern">The search string to match against the names of directories.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
    /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
    /// <returns>An enumerable collection of file system information objects that matches <paramref name="searchPattern" />.</returns>
    IEnumerable<IPathInfo> EnumeratePathInfos(string searchPattern);
    /// <summary>Returns an enumerable collection of file system information that matches a specified search pattern and search subdirectory option.</summary>
    /// <param name="searchPattern">The search string to match against the names of directories.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
    /// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories. The default value is <see cref="F:System.IO.SearchOption.TopDirectoryOnly" />.</param>
    /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
    /// <returns>An enumerable collection of file system information objects that matches <paramref name="searchPattern" /> and <paramref name="searchOption" />.</returns>
    IEnumerable<IPathInfo> EnumeratePathInfos(string searchPattern, SearchOption searchOption);
}
