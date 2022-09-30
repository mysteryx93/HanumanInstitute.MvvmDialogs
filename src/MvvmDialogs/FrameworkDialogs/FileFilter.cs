using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs;

/// <summary>
/// Represents a filter in an OpenFileDialog or SaveFileDialog.
/// </summary>
public class FileFilter
{
    /// <summary>
    /// Initializes a new instance of the FileFilter class.
    /// </summary>
    public FileFilter()
    {
    }

    /// <summary>
    /// Initializes a new instance of the FileFilter class.
    /// </summary>
    /// <param name="name">The display name of the filter.</param>
    /// <param name="extension">The file extension to filter on, excluding the trailing dot.</param>
    public FileFilter(string name, string extension)
    {
        Name = name;
        Extensions = new[] { extension };
    }

    /// <summary>
    /// Initializes a new instance of the FileFilter class.
    /// </summary>
    /// <param name="name">The display name of the filter.</param>
    /// <param name="extensions">The file extensions to filter on, excluding the trailing dots.</param>
    public FileFilter(string name, IReadOnlyList<string> extensions)
    {
        Name = name;
        Extensions = extensions;
    }

    /// <summary>
    /// Gets or sets the display name of the filter.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a list of file extensions matched by the filter, excluding the trailing dots (e.g. "txt" or "*" for all files).
    /// </summary>
    /// <remarks>
    /// Used on Windows and Linux systems.
    /// </remarks>
    public IReadOnlyList<string>? Extensions { get; set; }

    /// <summary>
    /// List of extensions in MIME format.
    /// </summary>
    /// <remarks>
    /// Used on Android, Browser and Linux systems.
    /// </remarks>
    public IReadOnlyList<string>? MimeTypes { get; set; }

    /// <summary>
    /// List of extensions in Apple uniform format.
    /// </summary>
    /// <remarks>
    /// Used only on Apple devices.
    /// See https://developer.apple.com/documentation/uniformtypeidentifiers/system_declared_uniform_type_identifiers.
    /// </remarks>
    public IReadOnlyList<string>? AppleUniformTypeIdentifiers { get; set; }

    /// <summary>
    /// Returns a string with all extensions starting with '*.' and separated by specified separator.
    /// ex: "*.BMP;*.JPG;*.GIF"
    /// </summary>
    /// <param name="separator">The separator between extensions.</param>
    /// <returns>A string representation of the extensions.</returns>
    public string? ExtensionsToString(char separator = ';')
    {
        if (Extensions == null) { return null; }
        
        var builder = new StringBuilder();
        foreach (var ext in Extensions.Select(x => x.TrimStart('.')))
        {
            // Add separator.
            if (builder.Length > 0)
            {
                builder.Append(separator);
            }

            // Add *.ext
            builder.Append('*');
            if (!string.IsNullOrEmpty(ext) && !ext.StartsWith("'"))
            {
                builder.Append('.');
            }
            builder.Append(ext);
        }
        return builder.ToString();
    }

    /// <summary>
    /// Returns a string containing the name plus extensions.
    /// ex: "Image Files (*.BMP;*.JPG;*.GIF)"
    /// </summary>
    /// <remarks>
    /// The '.' in extensions is optional. Extensions will automatically be added
    /// to the descriptions unless it contains '('.
    /// If you do not wish to display extensions, end the name with '()' and it will be trimmed away.
    /// </remarks>
    /// <param name="extensions">The extensions to add, calculated with <see cref="ExtensionsToString"/>.</param>
    /// <returns>The name ready for display.</returns>
    public string NameToString(string? extensions)
    {
        var name = Name ?? string.Empty;
        // Only add extensions to description if it doesn't contain parenthesis.
        var hasExtInDesc = name.Contains("(");
        // If name ends with '()', trim it and display no extensions.
        if (name.EndsWith("()"))
        {
            name = name.Substring(0, name.Length - 2).TrimEnd();
        }

        // Add all extensions to description.
        if (!hasExtInDesc && !string.IsNullOrEmpty(extensions))
        {
            name += " (" + extensions + ")";
        }
        return name;
    }
}
