namespace Shiny.Maui.Controls.Chat;

/// <summary>
/// Base class for bubble tools that act on a specific ChatMessage.
/// The Message property is automatically populated when the tool menu opens.
/// Subclass this to create self-contained bubble tools without needing ViewModel commands.
/// </summary>
public class ChatBubbleTool : FabMenuItem
{
    /// <summary>
    /// The ChatMessage this tool is acting on. Set automatically by ChatView
    /// when the bubble tool menu opens (via CommandParameter).
    /// </summary>
    protected ChatMessage? Message => CommandParameter as ChatMessage;

    /// <summary>
    /// The parent ChatView. Set automatically when the bubble tool menu opens.
    /// </summary>
    internal ChatView? ParentChatView { get; set; }

    /// <summary>
    /// Requests the parent ChatView to refresh its message bubbles.
    /// Call this after modifying message data (e.g. adding acknowledgements).
    /// </summary>
    protected void RequestRefresh() => ParentChatView?.RefreshBubbles();
}
