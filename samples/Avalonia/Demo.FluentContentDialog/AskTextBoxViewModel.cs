using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Demo.Avalonia.FluentContentDialog;

public partial class AskTextBoxViewModel : ViewModelBase
{
    [Reactive]
    public partial string Title { get; set; } = "Title";

    [Reactive]
    public partial string Text { get; set; } = string.Empty;
}
