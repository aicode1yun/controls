namespace Shiny.Maui.Controls.Chat;

/// <summary>
/// Base class for input bar tools that need access to the ChatView.
/// Provides the ChatView reference automatically via Attach/Detach lifecycle.
/// Can be used directly in XAML with a Command binding, or subclassed for self-contained tools.
/// </summary>
public class ChatEntryTool : FabMenuItem
{
    protected ChatView? ChatView { get; private set; }

    internal void Attach(ChatView chatView) => ChatView = chatView;
    internal void Detach() => ChatView = null;
}
