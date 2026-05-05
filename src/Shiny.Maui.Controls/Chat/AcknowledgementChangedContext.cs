namespace Shiny.Maui.Controls.Chat;

/// <summary>
/// Context passed when an acknowledgement is toggled on a message.
/// </summary>
public class AcknowledgementChangedContext
{
    public AcknowledgementChangedContext(ChatMessage message, string glyph)
    {
        Message = message;
        Glyph = glyph;
    }

    public ChatMessage Message { get; }
    public string Glyph { get; }
}
