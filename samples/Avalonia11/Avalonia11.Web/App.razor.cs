using Avalonia.Web.Blazor;

namespace Avalonia11.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        WebAppBuilder.Configure<Avalonia11.App>()
            .SetupWithSingleViewLifetime();
    }
}
