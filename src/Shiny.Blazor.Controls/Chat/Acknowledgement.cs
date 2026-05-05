namespace Shiny.Blazor.Controls.Chat;

public class Acknowledgement
{
    public string? Glyph { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
