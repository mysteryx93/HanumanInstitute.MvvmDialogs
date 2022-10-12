using Avalonia.Web.Blazor;

namespace Demo.CrossPlatform.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        WebAppBuilder.Configure<Demo.CrossPlatform.App>()
            .SetupWithSingleViewLifetime();
    }
}
