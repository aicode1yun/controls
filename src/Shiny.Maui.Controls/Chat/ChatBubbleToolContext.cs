namespace Shiny.Maui.Controls.Chat;

/// <summary>
/// Context passed when a bubble tool item is tapped,
/// providing both the originating message and the tapped menu item.
/// </summary>
public class ChatBubbleToolContext
{
    public ChatBubbleToolContext(ChatMessage message, FabMenuItem item)
    {
        Message = message;
        Item = item;
    }

    public ChatMessage Message { get; }
    public FabMenuItem Item { get; }
}
