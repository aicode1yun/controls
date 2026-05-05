namespace Shiny.Blazor.Controls.Chat;

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
    /// The date/time the message was confirmed sent to the server.
    /// When null, the bubble is rendered with reduced opacity (pending/offline). Only applies to user messages (IsFromMe).
    /// </summary>
    public DateTimeOffset? DateSent { get; set; }

    /// <summary>
    /// Acknowledgements (reactions) on this message. Displayed as grouped badges below the bubble.
    /// </summary>
    public List<Acknowledgement>? Acknowledgements { get; set; }
}
