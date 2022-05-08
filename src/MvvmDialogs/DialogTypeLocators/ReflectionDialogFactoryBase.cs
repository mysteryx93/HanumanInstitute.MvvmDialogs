using System;

namespace HanumanInstitute.MvvmDialogs.DialogTypeLocators;

/// <summary>
/// Class responsible for creating dialogs using reflection.
/// </summary>
/// <typeparam name="T">The native type of windows for the target framework.</typeparam>
public abstract class ReflectionDialogFactoryBase<T> : IDialogFactory
{
    /// <inheritdoc />
    public IWindow Create(Type dialogType)
    {
        if (dialogType == null) throw new ArgumentNullException(nameof(dialogType));

        var instance = Activator.CreateInstance(dialogType);
        return instance switch
        {
            IWindow w => w,
            T w => CreateWrapper(w),
            _ => throw new ArgumentException($"Only dialogs of type {typeof(T)} are supported.")
        };
    }

    /// <summary>
    /// Creates a wrapper around a native window.
    /// </summary>
    /// <param name="window">The window to create a wrapper for.</param>
    protected abstract IWindow CreateWrapper(T window);
}
