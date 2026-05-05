namespace Shiny.Maui.Controls.Chat;

public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Text { get; set; }
    public string? ImageUrl { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    public bool IsFromMe { get; set; }

    /// <summary>
    /// Optional identifier for attaching additional context to a message after it is sent.
    /// </summary>
    public string? Identifier { get; set; }

    /// <summary>
    /// Indicates whether the message has been confirmed sent to the server.
    /// When false, the bubble is rendered with reduced opacity. Only applies to user messages (IsFromMe).
    /// </summary>
    public bool IsSent { get; set; }

    /// <summary>
    /// Acknowledgements (reactions) on this message. Displayed as grouped badges below the bubble.
    /// </summary>
    public List<Acknowledgement>? Acknowledgements { get; set; }

    /// <summary>
    /// Tool actions displayed as a FabMenu next to the chat bubble.
    /// When set, a small tools button appears beside the bubble.
    /// </summary>
    public IList<FabMenuItem>? ToolItems { get; set; }

    /// <summary>
    /// Internal flag used by ChatView to render this message as a typing indicator bubble.
    /// </summary>
    internal bool IsTypingIndicator { get; set; }
}
