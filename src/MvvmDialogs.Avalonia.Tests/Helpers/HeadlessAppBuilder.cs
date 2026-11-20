namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

/// <summary>
/// Configures the Avalonia dispatcher for tests that use controls.
/// </summary>
public static class HeadlessAppBuilder
{
    /// <summary>
    /// Builds the application with a headless platform for UI-thread tests.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<Application>().UseHeadless(new());
}
