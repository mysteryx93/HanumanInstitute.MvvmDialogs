using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace Demo.CrossPlatform.Android;

[Activity(Label = "Avalonia11.Android", Theme = "@style/MyTheme.NoActionBar", Icon = "@drawable/icon", LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
public class MainActivity : AvaloniaMainActivity
{
}
