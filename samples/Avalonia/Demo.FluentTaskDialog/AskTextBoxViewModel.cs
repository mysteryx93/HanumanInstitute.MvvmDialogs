using ReactiveUI.SourceGenerators;

namespace Demo.Avalonia.FluentTaskDialog;

public partial class AskTextBoxViewModel : ViewModelBase
{
    [Reactive]
    public partial string Title { get; set; } = "Title";

    [Reactive]
    public partial string Text { get; set; } = string.Empty;
}
