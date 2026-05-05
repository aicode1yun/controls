namespace Shiny.Maui.Controls.Chat;

/// <summary>
/// A pre-built bubble tool that copies the message text (or image URL) to the clipboard.
/// Drop into BubbleToolItems without needing a ViewModel command.
/// </summary>
public class CopyBubbleTool : ChatBubbleTool
{
    public CopyBubbleTool()
    {
        Text = "Copy";
        FabBackgroundColor = Color.FromArgb("#607D8B");
        Clicked += OnClicked;
    }

    void OnClicked(object? sender, EventArgs e)
    {
        if (Message is null)
            return;

        var text = !string.IsNullOrEmpty(Message.Text)
            ? Message.Text
            : Message.ImageUrl;

        if (!string.IsNullOrEmpty(text))
            Clipboard.Default.SetTextAsync(text).FireAndForget();
    }
}

file static class TaskExtensions
{
    public static async void FireAndForget(this Task task)
    {
        try { await task; }
        catch { /* swallow in fire-and-forget */ }
    }
}
