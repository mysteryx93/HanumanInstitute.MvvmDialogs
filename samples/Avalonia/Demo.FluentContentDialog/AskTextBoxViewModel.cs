using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Demo.Avalonia.FluentContentDialog;

public partial class AskTextBoxViewModel : ViewModelBase
{
    [Reactive]
    public string Title { get; set; } = "Title";

    [Reactive]
    public string Text { get; set; } = string.Empty;
}
